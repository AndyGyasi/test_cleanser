namespace CleanserBlazorUI.Repository;
public class DataCleaningType
{
    public List<DataCleaningTypeModel>? GetDataCleanings()
    {
        var getDataCleanings = new List<DataCleaningTypeModel>()
        {
            new DataCleaningTypeModel { Id= 1, IdShortName = "ind", Title = "1. INDIVIDUAL RECORDS IND"},
            new DataCleaningTypeModel { Id= 2, IdShortName = "bus",  Title = "2. BUSINESS RECORDS BUS"},
            new DataCleaningTypeModel { Id= 3, IdShortName = "idu",  Title = "3. INDIVIDUAL RECORDS DUD"},
            new DataCleaningTypeModel { Id= 4, IdShortName = "bdu",  Title = "4. BUSINESS RECORDS DUD"},
            new DataCleaningTypeModel { Id= 5, IdShortName = "iju",  Title = "5. INDIVIDUAL RECORDS (JUD)"},
            new DataCleaningTypeModel { Id= 6, IdShortName = "bju",  Title = "6. BUSINESS RECORDS (JUD)"},
        };
        return getDataCleanings;
    }
    public List<DataCleaningTypeModel>? GetDataReferencings()
    {
        var getDataCleanings = new List<DataCleaningTypeModel>()
        {
            new DataCleaningTypeModel { Id= 1, IdShortName = "ind", Title = "1. INDIVIDUAL RECORDS IND (REFERENCING)"},
            new DataCleaningTypeModel { Id= 2, IdShortName = "bus",  Title = "2. BUSINESS RECORDS BUS (REFERENCING)"},
            new DataCleaningTypeModel { Id= 3, IdShortName = "idu",  Title = "3. INDIVIDUAL RECORDS DUD (REFERENCING)"},
            new DataCleaningTypeModel { Id= 4, IdShortName = "bdu",  Title = "4. BUSINESS RECORDS DUD (REFERENCING)"},
        };
        return getDataCleanings;
    }
}
public class DataCleaningTypeModel
{
    public int Id { get; set; }
    public string IdShortName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsExpanded { get; set; } = false;
}