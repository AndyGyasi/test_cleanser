using DocumentFormat.OpenXml.Wordprocessing;

namespace CleanserBlazorUI.Entities;
public class BusinessContext
{
    public string CustomerIDBusregnumTinumStatus { get; set; } = string.Empty;
    /// <summary>
    /// Set when a shared Busregnum/Tinum/CustomerID's business names only
    /// agree via fuzzy (token/abbreviation) matching, not an exact string
    /// match -- e.g. "ASSOCIATION INTERNATIONAL SCHOOL" vs "...SCH", or
    /// "...SCHOOL" vs "...SCHOOL-LOAN ACCOUNT". Not a conflict (record stays
    /// clean), but worth a note distinct from a hard error.
    /// </summary>
    public string? BusinessNameFuzzyMatchNote { get; set; }
    /// <summary>Set to "UNL" when the same AccNum+CustomerID appears with a
    /// different DisbursementDate elsewhere in the same submission -- exempted
    /// for overdraft facilities (CreditFacilityType == "V"), which legitimately
    /// redraw on new dates. Mirrors IndividualContext.DisbursementDateMisMatch.</summary>
    public string DisbursementDateMisMatch { get; set; } = string.Empty;
    public string DisburStatus { get; set; }
    public string Status { get; set; }
    public string DateOfBirth { get; set; } = string.Empty;
    public string UNL_writtenoffamt { get; set; } = string.Empty;
    public string UNL_Busregnum { get; set; } = string.Empty;
    public string UNL_Tinum { get; set; } = string.Empty;
    public string UNL_NDIA_OldData { get; set; } = string.Empty;
    public string UNL_otheridtype { get; set; } = string.Empty;
    public string UNL_otheridnum { get; set; } = string.Empty;
    public string UNL_proofofaddtype { get; set; } = string.Empty;
    public string UNL_proofofaddnum { get; set; } = string.Empty;
    public bool IsPassed { get; set; } = false;
    public string Data { get; set; } = "D";
    //public CellDataAndStatus? CorrectionIndicator { get; set; } = new CellDataAndStatus(string.Empty);
    public string CorrectionIndicator { get; set; } = string.Empty;
    public CellDataAndStatus Facilityaccnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CustomerID { get; set; } = new CellDataAndStatus(string.Empty);
    //public CellDataAndStatus? BranchCode { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus BranchCode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Busregnum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Prevregnum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Tinum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Sectorindcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Subsecindcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Bustype { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Registrationdate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Commencementdate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Businessname { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Tradingname { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Turnovercurrency { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Turnoveramount { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Prevbusname { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus ProofOfAddType { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus ProofOfAddNum { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd1 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd2 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd3 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Curlocadd4 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Curlocaddrpostalcode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? PostAddrLine1 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? PostAddrLine2 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? PostAddrLine3 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? PostAddrLine4 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus PostalAddPostCode { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Websiteadd { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus EmailAddress { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Officetel1 { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? Officetel2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Officefaxnum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus OldCustomerID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus OldAccountNum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus OldSRN { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus OldBranchCode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CreditFacilityType { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? PurposeOfFacility { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? FacilityTerm { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus DefPaymentStartDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? AmountCurrency { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? FacilityAmount { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? DisbursementDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? DisbursementAmt { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? MaturityDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? SchdInstalAmount { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? RepaymentFreq { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? LastPaymentAmount { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? LastPaymentDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? NextPaymentDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CurBal { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CurBalIndicator { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AssetClassification { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? AmountInArrears { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus ArrearsStartDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? NDIA { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus PaymentHistoryProfile { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AmtOverdue1to30days { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AmtOverdue31to60days { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AmtOverdue61to90days { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AmtOverdue91to120days { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Amtoverdue121To150Days { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AmtOverdue151to180days { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus AmtOverdue181orMore { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus LegalFlag { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? FacilityStatusCode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? FacilityStatusDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? ClosedDate { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? ClosureReason { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? WrittenOffAmt { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus ReasonForWrittenOff { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? DateRestructured { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? ReasonForRestructure { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? CreditCollateralInd { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus SecurityType { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NatureOfCharge { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus SecurityValue { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus CollRegRefNum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus SpecialCommentsCode { get; set; } = new CellDataAndStatus(string.Empty);
    public CellDataAndStatus? NatureOfGuarantor1 { get; set; } = new CellDataAndStatus(string.Empty);



    public  CellDataAndStatus NameOfComGuarantor1 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus BusRegOfGuarantor1 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1Surname { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1FirstName { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1MiddleNames { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1NatID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1VotID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1DrivLic { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1PassNum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1SSN { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1Gender { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1DOB { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1Add1 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1Add2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1Add3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1HomeTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1WorkTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G1Mobile { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NatureOfGuarantor2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NameOfComGuarantor2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus Busregofguarantor2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2Surname { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2FirstName { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2MiddleNames { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2NatID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2VotID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2DrivLic { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2PassNum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2SSN { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2Gender { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2DOB { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2Add1 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2Add2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2Add3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2HomeTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2WorkTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G2Mobile { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NatureOfGuarantor3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NameOfComGuarantor3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus BusRegOfGuarantor3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3Surname { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3FirstName { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3MiddleNames { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3NatID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3VotID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3DrivLic { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3PassNum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3SSN { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3Gender { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3DOB { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3Add1 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3Add2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3Add3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3HomeTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3WorkTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G3Mobile { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NatureOfGuarantor4 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus NameOfComGuarantor4 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus BusRegOfGuarantor4 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4Surname { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4FirstName { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4MiddleNames { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4NatID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4VotID { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4DrivLic { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4PassNum { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4SSN { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4Gender { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4DOB { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4Add1 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4Add2 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4Add3 { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4HomeTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4WorkTel { get; set; } = new CellDataAndStatus(string.Empty);
    public  CellDataAndStatus G4Mobile { get; set; } = new CellDataAndStatus(string.Empty);
}
 