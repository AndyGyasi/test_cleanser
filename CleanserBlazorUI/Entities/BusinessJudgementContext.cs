namespace CleanserBlazorUI.Entities;
public class BusinessJudgementContext
{
    public string Status { get; set; }
    public bool IsPassed { get; set; } = false;
    public string? Data { get; set; } = "D";
    public string? Correctionindicator { get; set; } = string.Empty;
    public CellDataAndStatus? Facilityaccnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CustomerID { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Branchcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Busregnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Prevregnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Tinum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Sectorindcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Subsecindcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Bustype { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Registrationdate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Commencementdate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Businessname { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Tradingname { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Turnovercurrency { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Turnoveramount { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Prevbusname { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Proofofaddtype { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Proofofaddnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd1 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd2 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd3 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd4 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocaddrpostalcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Postaddrline1 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Postaddrline2 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Postaddrline3 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Postaddrline4 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Postaladdpostcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Websiteadd { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Emailaddress { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Officetel1 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Officetel2 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Officefaxnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Oldcustomerid { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Oldaccountnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Oldsrn { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Oldbranchcode { get; set; } = new CellDataAndStatus(string.Empty);
    // Court/case fields -- left exactly as submitted, no cleaning applied.
    public CellDataAndStatus? CourtName { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CourtLocation { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CourtType { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CaseNumber { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CaseFilingDate { get; set; } = new CellDataAndStatus(string.Empty);
    // Fixed values in CLEAN output, regardless of raw input.
    public CellDataAndStatus? CaseType { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CaseReason { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? AmountCurrency { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Amount { get; set; } = new CellDataAndStatus(string.Empty);
}
