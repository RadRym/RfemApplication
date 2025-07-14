using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RfemApplication.Models;
using Dlubal.WS.Rfem6.Model;
using System.Linq;

namespace RfemApplication.Services
{
    public interface IRfemElementsService
    {
        Task<List<RfemElement>> GetElementsAsync(IRfemModel modelClient);
        Task<List<RfemMaterial>> GetMaterialsAsync(IRfemModel modelClient);
        Task<List<RfemCrossSection>> GetCrossSectionsAsync(IRfemModel modelClient);
    }

    public class RfemElementsService : IRfemElementsService
    {
        public async Task<List<RfemElement>> GetElementsAsync(IRfemModel modelClient)
        {
            if (modelClient == null)
                throw new ArgumentNullException(nameof(modelClient));

            var elements = new List<RfemElement>();

            try
            {
                // 1. Pobierz wszystkie numery obiektów typu "member" - użyj prawidłowego enum
                var allObjectsRequest = new get_all_object_numbers_by_typeRequest
                {
                    type = object_types.E_OBJECT_TYPE_MEMBER
                };

                var objectsResponse = await Task.Run(() =>
                    modelClient.get_all_object_numbers_by_type(allObjectsRequest));

                if (objectsResponse?.value == null || objectsResponse.value.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Brak elementów w modelu RFEM");
                    return elements;
                }

                System.Diagnostics.Debug.WriteLine($"Znaleziono {objectsResponse.value.Length} elementów");

                // 2. Pobierz materiały i przekroje (dla referencji)
                var materials = await GetMaterialsAsync(modelClient);
                var crossSections = await GetCrossSectionsAsync(modelClient);

                // 3. Dla każdego elementu pobierz szczegóły
                foreach (var memberId in objectsResponse.value)
                {
                    try
                    {
                        var memberRequest = new get_memberRequest();
                        memberRequest.no = memberId.no;

                        var memberResponse = await Task.Run(() =>
                            modelClient.get_member(memberRequest));

                        if (memberResponse?.value != null)
                        {
                            var member = memberResponse.value;

                            // Stwórz element na podstawie danych z RFEM
                            var element = new RfemElement
                            {
                                ID = (int)member.no,
                                ElementType = GetElementTypeFromMember(member),
                                Length = await CalculateMemberLengthAsync(member, modelClient),
                                CrossSection = GetCrossSectionName(member.section_start, crossSections),
                                Material = GetMaterialName(member.section_material, materials),
                                StartNode = await GetStartNodeNameAsync(member, modelClient),
                                EndNode = await GetEndNodeNameAsync(member, modelClient),
                                LoadCase = "Statyczny", // Domyślny przypadek
                                AxialForce = 0.0,
                                ShearForce = 0.0,
                                BendingMoment = 0.0
                            };

                            elements.Add(element);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Błąd pobierania elementu {memberId}: {ex.Message}");
                        // Kontynuuj z następnym elementem
                    }
                }

                System.Diagnostics.Debug.WriteLine($"Pomyślnie pobrano {elements.Count} elementów");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd podczas pobierania elementów: {ex.Message}");
                throw new Exception($"Błąd pobierania elementów z RFEM: {ex.Message}", ex);
            }

            return elements;
        }

        public async Task<List<RfemMaterial>> GetMaterialsAsync(IRfemModel modelClient)
        {
            var materials = new List<RfemMaterial>();

            try
            {
                var materialsRequest = new get_all_object_numbers_by_typeRequest
                {
                    type = object_types.E_OBJECT_TYPE_MATERIAL
                };

                var materialsResponse = await Task.Run(() =>
                    modelClient.get_all_object_numbers_by_type(materialsRequest));

                if (materialsResponse?.value != null)
                {
                    foreach (var materialId in materialsResponse.value)
                    {
                        try
                        {
                            var materialRequest = new get_materialRequest();
                            materialRequest.no = materialId.no;

                            var materialResponse = await Task.Run(() =>
                                modelClient.get_material(materialRequest));

                            if (materialResponse?.value != null)
                            {
                                var material = materialResponse.value;
                                materials.Add(new RfemMaterial
                                {
                                    ID = (int)material.no,
                                    Name = material.name ?? $"Material_{material.no}",
                                    Type = GetMaterialType(material),
                                    ElasticModulus = GetElasticModulus(material),
                                    Density = GetDensity(material),
                                    PoissonRatio = GetPoissonRatio(material)
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd pobierania materiału {materialId}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania materiałów: {ex.Message}");
            }

            return materials;
        }

        public async Task<List<RfemCrossSection>> GetCrossSectionsAsync(IRfemModel modelClient)
        {
            var crossSections = new List<RfemCrossSection>();

            try
            {
                var sectionsRequest = new get_all_object_numbers_by_typeRequest
                {
                    type = object_types.E_OBJECT_TYPE_SECTION
                };

                var sectionsResponse = await Task.Run(() =>
                    modelClient.get_all_object_numbers_by_type(sectionsRequest));

                if (sectionsResponse?.value != null)
                {
                    foreach (var sectionId in sectionsResponse.value)
                    {
                        try
                        {
                            var sectionRequest = new get_sectionRequest();
                            sectionRequest.no = sectionId.no;

                            var sectionResponse = await Task.Run(() =>
                                modelClient.get_section(sectionRequest));

                            if (sectionResponse?.value != null)
                            {
                                var section = sectionResponse.value;
                                crossSections.Add(new RfemCrossSection
                                {
                                    ID = (int)section.no,
                                    Name = section.name ?? $"Section_{section.no}",
                                    Type = GetSectionType(section),
                                    Area = GetSectionArea(section),
                                    MomentOfInertiaY = GetMomentOfInertiaY(section),
                                    MomentOfInertiaZ = GetMomentOfInertiaZ(section),
                                    Height = GetSectionHeight(section),
                                    Width = GetSectionWidth(section)
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Błąd pobierania przekroju {sectionId}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania przekrojów: {ex.Message}");
            }

            return crossSections;
        }

        #region Helper Methods

        private string GetElementTypeFromMember(member _member)
        {
            // Analiza typu elementu na podstawie danych z RFEM
            try
            {
                // W przyszłości można analizować właściwości member'a
                // np. sprawdzać member.type jeśli dostępne
                return "Beam"; // Domyślnie
            }
            catch
            {
                return "Member";
            }
        }

        private async Task<double> CalculateMemberLengthAsync(member _member, IRfemModel modelClient)
        {
            try
            {
                // Pobierz węzły początkowy i końcowy
                if (_member.line != null)
                {
                    var lineRequest = new get_lineRequest();
                    lineRequest.no = _member.no;

                    var lineResponse = await Task.Run(() => modelClient.get_line(lineRequest));
                    if (lineResponse?.value != null)
                    {
                        var line = lineResponse.value;
                        // Oblicz długość na podstawie współrzędnych węzłów
                        return await CalculateLineLengthAsync(line, modelClient);
                    }
                }
                return 5.0; // Wartość domyślna
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd obliczania długości elementu: {ex.Message}");
                return 0.0;
            }
        }

        private async Task<double> CalculateLineLengthAsync(line _line, IRfemModel modelClient)
        {
            try
            {
                // Pobierz współrzędne węzłów początkowego i końcowego
                if (_line.definition_nodes != null && _line.definition_nodes.Length >= 2)
                {
                    var startNodeId = _line.definition_nodes[0];
                    var endNodeId = _line.definition_nodes[_line.definition_nodes.Length - 1];

                    // Pobierz współrzędne węzła początkowego
                    var startNodeRequest = new get_nodeRequest();
                    startNodeRequest.no = startNodeId;
                    var startNodeResponse = await Task.Run(() => modelClient.get_node(startNodeRequest));

                    // Pobierz współrzędne węzła końcowego
                    var endNodeRequest = new get_nodeRequest();
                    endNodeRequest.no = endNodeId;
                    var endNodeResponse = await Task.Run(() => modelClient.get_node(endNodeRequest));

                    if (startNodeResponse?.value != null && endNodeResponse?.value != null)
                    {
                        var startNode = startNodeResponse.value;
                        var endNode = endNodeResponse.value;

                        // Oblicz długość euklidesową
                        double dx = endNode.coordinate_1 - startNode.coordinate_1;
                        double dy = endNode.coordinate_2 - startNode.coordinate_2;
                        double dz = endNode.coordinate_3 - startNode.coordinate_3;

                        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    }
                }
                return 5.0; // Wartość domyślna
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd obliczania długości linii: {ex.Message}");
                return 5.0; // Wartość domyślna
            }
        }

        private string GetCrossSectionName(int sectionId, List<RfemCrossSection> crossSections)
        {
            var section = crossSections.FirstOrDefault(s => s.ID == sectionId);
            return section?.Name ?? $"Section_{sectionId}";
        }

        private string GetMaterialName(int materialId, List<RfemMaterial> materials)
        {
            var material = materials.FirstOrDefault(m => m.ID == materialId);
            return material?.Name ?? $"Material_{materialId}";
        }

        private async Task<string> GetStartNodeNameAsync(member _member, IRfemModel modelClient)
        {
            try
            {
                if (_member.line != null)
                {
                    var lineRequest = new get_lineRequest();
                    lineRequest.no = _member.no;

                    var lineResponse = await Task.Run(() => modelClient.get_line(lineRequest));
                    if (lineResponse?.value != null && lineResponse.value.definition_nodes != null && lineResponse.value.definition_nodes.Length > 0)
                    {
                        return $"N{lineResponse.value.definition_nodes[0]}";
                    }
                }
                return "N1"; // Domyślnie
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania węzła początkowego: {ex.Message}");
                return "N?";
            }
        }

        private async Task<string> GetEndNodeNameAsync(member _member, IRfemModel modelClient)
        {
            try
            {
                if (_member.line != null)
                {
                    var lineRequest = new get_lineRequest();
                    lineRequest.no = _member.no;

                    var lineResponse = await Task.Run(() => modelClient.get_line(lineRequest));
                    if (lineResponse?.value != null && lineResponse.value.definition_nodes != null && lineResponse.value.definition_nodes.Length > 0)
                    {
                        var lastIndex = lineResponse.value.definition_nodes.Length - 1;
                        return $"N{lineResponse.value.definition_nodes[lastIndex]}";
                    }
                }
                return "N2"; // Domyślnie
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd pobierania węzła końcowego: {ex.Message}");
                return "N?";
            }
        }

        // Helper methods for material properties
        private string GetMaterialType(material _material)
        {
            try
            {
                // Analiza typu materiału na podstawie właściwości
                // W przyszłości można analizować _material.material_type lub inne właściwości
                return "Steel"; // Domyślnie
            }
            catch
            {
                return "Unknown";
            }
        }

        private double GetElasticModulus(material _material)
        {
            try
            {
                // Pobranie modułu Younga z właściwości materiału
                // W przyszłości można odczytać z _material.E jeśli dostępne
                return 210000; // Domyślnie dla stali (MPa)
            }
            catch
            {
                return 0.0;
            }
        }

        private double GetDensity(material _material)
        {
            try
            {
                // Pobranie gęstości z właściwości materiału
                // W przyszłości można odczytać z _material.specific_weight jeśli dostępne
                return 7850; // Domyślnie dla stali (kg/m³)
            }
            catch
            {
                return 0.0;
            }
        }

        private double GetPoissonRatio(material _material)
        {
            try
            {
                // Pobranie współczynnika Poissona
                // W przyszłości można odczytać z _material.nu jeśli dostępne
                return 0.3; // Domyślnie dla stali
            }
            catch
            {
                return 0.0;
            }
        }

        // Helper methods for section properties
        private string GetSectionType(section _section)
        {
            try
            {
                // Analiza typu przekroju
                // W przyszłości można analizować _section.type jeśli dostępne
                return "I-Section"; // Domyślnie
            }
            catch
            {
                return "Unknown";
            }
        }

        private double GetSectionArea(section _section)
        {
            try
            {
                // Pobranie pola przekroju
                // W przyszłości można odczytać z _section.A jeśli dostępne
                return 0.01; // Domyślnie (m²)
            }
            catch
            {
                return 0.0;
            }
        }

        private double GetMomentOfInertiaY(section _section)
        {
            try
            {
                // Pobranie momentu bezwładności względem osi Y
                // W przyszłości można odczytać z _section.Iy jeśli dostępne
                return 0.0001; // Domyślnie (m⁴)
            }
            catch
            {
                return 0.0;
            }
        }

        private double GetMomentOfInertiaZ(section _section)
        {
            try
            {
                // Pobranie momentu bezwładności względem osi Z
                // W przyszłości można odczytać z _section.Iz jeśli dostępne
                return 0.0001; // Domyślnie (m⁴)
            }
            catch
            {
                return 0.0;
            }
        }

        private double GetSectionHeight(section _section)
        {
            try
            {
                // Pobranie wysokości przekroju
                // W przyszłości można odczytać z _section.h jeśli dostępne
                return 300; // Domyślnie (mm)
            }
            catch
            {
                return 0.0;
            }
        }

        private double GetSectionWidth(section _section)
        {
            try
            {
                // Pobranie szerokości przekroju
                // W przyszłości można odczytać z _section.b jeśli dostępne
                return 150; // Domyślnie (mm)
            }
            catch
            {
                return 0.0;
            }
        }

        #endregion
    }
}