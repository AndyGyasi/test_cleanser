using DocumentFormat.OpenXml.Office2010.Excel;

namespace CleanserBlazorUI.Entities;
public class DBIndividualContext 
{
    public int Id { get; set; }
    public int CurrenVersion { get; set; }
    public string SubscriberCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public string CorrectionIndicator { get; set; } = string.Empty;
    public string? CreditFacilityAccNum { get; set; } = string.Empty;
    public string? CustomerID { get; set; } = string.Empty;
    public string? BranchCode { get; set; } = string.Empty;
    public string? NatIDNum { get; set; } = string.Empty;
    public string? VotersIDNum { get; set; } = string.Empty;
    public string? DriverLicNum { get; set; } = string.Empty;
    public string? PassportNum { get; set; } = string.Empty;
    public string? SSNum { get; set; } = string.Empty;
    public string? EzwichNum { get; set; } = string.Empty;
    public string? OtherIDType { get; set; } = string.Empty;
    public string? OtherIDNum { get; set; } = string.Empty;
    public string? TINum { get; set; } = string.Empty;
    public string? Gender { get; set; } = string.Empty;
    public string? MaritalStatus { get; set; } = string.Empty;
    public string? Nationality { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; } = string.Empty;
    public string? Title { get; set; } = string.Empty;
    public string? Surname { get; set; } = string.Empty;
    public string? FirstName { get; set; } = string.Empty;
    public string? MiddleNames { get; set; } = string.Empty;
    public string? PreviousNames { get; set; } = string.Empty;
    public string? Alias { get; set; } = string.Empty;
    public string? ProofOfAddType { get; set; } = string.Empty;
    public string? ProofOfAddNum { get; set; } = string.Empty;
    public string? CurResAddr1 { get; set; } = string.Empty;
    public string? CurResAddr2 { get; set; } = string.Empty;
    public string? CurResAddr3 { get; set; } = string.Empty;
    public string? CurResAddr4 { get; set; } = string.Empty;
    public string? CurResAddrPostalCode { get; set; } = string.Empty;
    public string? DateMovedCurrRes { get; set; } = string.Empty;
    public string? PrevResAddr1 { get; set; } = string.Empty;
    public string? PrevResAddr2 { get; set; } = string.Empty;
    public string? PrevResAddr3 { get; set; } = string.Empty;
    public string? PrevResAddr4 { get; set; } = string.Empty;
    public string? PrevResAddrPostalCode { get; set; } = string.Empty;
    public string? OwnerOrTenant { get; set; } = string.Empty;
    public string? PostAddrLine1 { get; set; } = string.Empty;
    public string? PostAddrLine2 { get; set; } = string.Empty;
    public string? PostAddrLine3 { get; set; } = string.Empty;
    public string? PostAddrLine4 { get; set; } = string.Empty;
    public string? PostalAddPostCode { get; set; } = string.Empty;
    public string? EmailAddress { get; set; } = string.Empty;
    public string? HomeTel { get; set; } = string.Empty;
    public string? MobileTel1 { get; set; } = string.Empty;
    public string? MobileTel2 { get; set; } = string.Empty;
    public string? WorkTel { get; set; } = string.Empty;
    public string? NumOfDependants { get; set; } = string.Empty;
    public string? EmpType { get; set; } = string.Empty;
    public string? EmpPayrollNum { get; set; } = string.Empty;
    public string? Paypoint { get; set; } = string.Empty;
    public string? EmpName { get; set; } = string.Empty;
    public string? EmpAddr1 { get; set; } = string.Empty;
    public string? EmpAddr2 { get; set; } = string.Empty;
    public string? EmpAddr3 { get; set; } = string.Empty;
    public string? EmpAddr4 { get; set; } = string.Empty;
    public string? EmpAddrPostalCode { get; set; } = string.Empty;
    public string? DateOfEmp { get; set; } = string.Empty;
    public string? Occupation { get; set; } = string.Empty;
    public string? IncomeCurrency { get; set; } = string.Empty;
    public string? Income { get; set; } = string.Empty;
    public string? JointOrSoleAcc { get; set; } = string.Empty;
    public string? NoParticipantsInAcc { get; set; } = string.Empty;
    public string? OldCustomerID { get; set; } = string.Empty;
    public string? OldAccountNum { get; set; } = string.Empty;
    public string? OldSRN { get; set; } = string.Empty;
    public string? OldBranchCode { get; set; } = string.Empty;
    public string? CreditFacilityType { get; set; } = string.Empty;
    public string? PurposeOfFacility { get; set; } = string.Empty;
    public string? FacilityTerm { get; set; } = string.Empty;
    public string? DefPaymentStartDate { get; set; } = string.Empty;
    public string? AmountCurrency { get; set; } = string.Empty;
    public string? FacilityAmount { get; set; } = string.Empty;
    public string? DisbursementDate { get; set; } = string.Empty;
    public string? DisbursementAmt { get; set; } = string.Empty;
    public string? MaturityDate { get; set; } = string.Empty;
    public string? SchdInstalAmount { get; set; } = string.Empty;
    public string? RepaymentFreq { get; set; } = string.Empty;
    public string? LastPaymentAmount { get; set; } = string.Empty;
    public string? LastPaymentDate { get; set; } = string.Empty;
    public string? NextPaymentDate { get; set; } = string.Empty;
    public string? CurBal { get; set; } = string.Empty;
    public string? CurBalIndicator { get; set; } = string.Empty;
    public string? AssetClassification { get; set; } = string.Empty;
    public string? AmountInArrears { get; set; } = string.Empty;
    public string? ArrearsStartDate { get; set; } = string.Empty;
    public string? NDIA { get; set; } = string.Empty;
    public string? PaymentHistoryProfile { get; set; } = string.Empty;
    public string? AmtOverdue1to30days { get; set; } = string.Empty;
    public string? AmtOverdue31to60days { get; set; } = string.Empty;
    public string? AmtOverdue61to90days { get; set; } = string.Empty;
    public string? AmtOverdue91to120days { get; set; } = string.Empty;
    public string? AmtOverdue121to150days { get; set; } = string.Empty;
    public string? AmtOverdue151to180days { get; set; } = string.Empty;
    public string? AmtOverdue181orMore { get; set; } = string.Empty;
    public string? LegalFlag { get; set; } = string.Empty;
    public string? FacilityStatusCode { get; set; } = string.Empty;
    public string? FacilityStatusDate { get; set; } = string.Empty;
    public string? ClosedDate { get; set; } = string.Empty;
    public string? ClosureReason { get; set; } = string.Empty;
    public string? WrittenOffAmt { get; set; } = string.Empty;
    public string? ReasonForWrittenOff { get; set; } = string.Empty;
    public string? DateRestructured { get; set; } = string.Empty;
    public string? ReasonForRestructure { get; set; } = string.Empty;
    public string? CreditCollateralInd { get; set; } = string.Empty;
    public string? SecurityType { get; set; } = string.Empty;
    public string? NatureOfCharge { get; set; } = string.Empty;
    public string? SecurityValue { get; set; } = string.Empty;
    public string? CollRegRefNum { get; set; } = string.Empty;
    public string? SpecialCommentsCode { get; set; } = string.Empty;
    public string? NatureOfGuarantor { get; set; } = string.Empty;
    public string? NameOfComGuarantor { get; set; } = string.Empty;
    public string? BusRegOfGuarantor { get; set; } = string.Empty;
    public string? G1Surname { get; set; } = string.Empty;
    public string? G1FirstName { get; set; } = string.Empty;
    public string? G1MiddleNames { get; set; } = string.Empty;
    public string? G1NatID { get; set; } = string.Empty;
    public string? G1VotID { get; set; } = string.Empty;
    public string? G1DrivLic { get; set; } = string.Empty;
    public string? G1PassNum { get; set; } = string.Empty;
    public string? G1SSN { get; set; } = string.Empty;
    public string? G1Gender { get; set; } = string.Empty;
    public string? G1DOB { get; set; } = string.Empty;
    public string? G1Add1 { get; set; } = string.Empty;
    public string? G1Add2 { get; set; } = string.Empty;
    public string? G1Add3 { get; set; } = string.Empty;
    public string? G1HomeTel { get; set; } = string.Empty;
    public string? G1WorkTel { get; set; } = string.Empty;
    public string? G1Mobile { get; set; } = string.Empty;
    public string? G2Surname { get; set; } = string.Empty;
    public string? G2FirstName { get; set; } = string.Empty;
    public string? G2MiddleNames { get; set; } = string.Empty;
    public string? G2NatID { get; set; } = string.Empty;
    public string? G2VotID { get; set; } = string.Empty;
    public string? G2DrivLic { get; set; } = string.Empty;
    public string? G2PassNum { get; set; } = string.Empty;
    public string? G2SSN { get; set; } = string.Empty;
    public string? G2Gender { get; set; } = string.Empty;
    public string? G2DOB { get; set; } = string.Empty;
    public string? G2Add1 { get; set; } = string.Empty;
    public string? G2Add2 { get; set; } = string.Empty;
    public string? G2Add3 { get; set; } = string.Empty;
    public string? G2HomeTel { get; set; } = string.Empty;
    public string? G2WorkTel { get; set; } = string.Empty;
    public string? G2Mobile { get; set; } = string.Empty;
    public string? G3Surname { get; set; } = string.Empty;
    public string? G3FirstName { get; set; } = string.Empty;
    public string? G3MiddleNames { get; set; } = string.Empty;
    public string? G3NatID { get; set; } = string.Empty;
    public string? G3VotID { get; set; } = string.Empty;
    public string? G3DrivLic { get; set; } = string.Empty;
    public string? G3PassNum { get; set; } = string.Empty;
    public string? G3SSN { get; set; } = string.Empty;
    public string? G3Gender { get; set; } = string.Empty;
    public string? G3DOB { get; set; } = string.Empty;
    public string? G3Add1 { get; set; } = string.Empty;
    public string? G3Add2 { get; set; } = string.Empty;
    public string? G3Add3 { get; set; } = string.Empty;
    public string? G3HomeTel { get; set; } = string.Empty;
    public string? G3WorkTel { get; set; } = string.Empty;
    public string? G3Mobile { get; set; } = string.Empty;
    public string? G4Surname { get; set; } = string.Empty;
    public string? G4FirstName { get; set; } = string.Empty;
    public string? G4MiddleNames { get; set; } = string.Empty;
    public string? G4NatID { get; set; } = string.Empty;
    public string? G4VotID { get; set; } = string.Empty;
    public string? G4DrivLic { get; set; } = string.Empty;
    public string? G4PassNum { get; set; } = string.Empty;
    public string? G4SSN { get; set; } = string.Empty;
    public string? G4Gender { get; set; } = string.Empty;
    public string? G4DOB { get; set; } = string.Empty;
    public string? G4Add1 { get; set; } = string.Empty;
    public string? G4Add2 { get; set; } = string.Empty;
    public string? G4Add3 { get; set; } = string.Empty;
    public string? G4HomeTel { get; set; } = string.Empty;
    public string? G4WorkTel { get; set; } = string.Empty;
    public string? G4Mobile { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
public class DBBusinessContext 
{
    public int Id { get; set; }
    public int CurrentVersion { get; set; }
    public string SubscriberCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CorrectionIndicator { get; set; } = string.Empty;
    public string Facilityaccnum { get; set; } = string.Empty;
    public string? CustomerID { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string? Busregnum { get; set; } = string.Empty;
    public string Prevregnum { get; set; } = string.Empty;
    public string? Tinum { get; set; } = string.Empty;
    public string? Sectorindcode { get; set; } = string.Empty;
    public string? Subsecindcode { get; set; } = string.Empty;
    public string? Bustype { get; set; } = string.Empty;
    public string? Registrationdate { get; set; } = string.Empty;
    public string? Commencementdate { get; set; } = string.Empty;
    public string? Businessname { get; set; } = string.Empty;
    public string? Tradingname { get; set; } = string.Empty;
    public string Turnovercurrency { get; set; } = string.Empty;
    public string Turnoveramount { get; set; } = string.Empty;
    public string Prevbusname { get; set; } = string.Empty;
    public string ProofOfAddType { get; set; } = string.Empty;
    public string ProofOfAddNum { get; set; } = string.Empty;
    public string? Curlocadd1 { get; set; } = string.Empty;
    public string? Curlocadd2 { get; set; } = string.Empty;
    public string? Curlocadd3 { get; set; } = string.Empty;
    public string? Curlocadd4 { get; set; } = string.Empty;
    public string Curlocaddrpostalcode { get; set; } = string.Empty;
    public string? PostAddrLine1 { get; set; } = string.Empty;
    public string? PostAddrLine2 { get; set; } = string.Empty;
    public string? PostAddrLine3 { get; set; } = string.Empty;
    public string? PostAddrLine4 { get; set; } = string.Empty;
    public string PostalAddPostCode { get; set; } = string.Empty;
    public string Websiteadd { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public string? Officetel1 { get; set; } = string.Empty;
    public string? Officetel2 { get; set; } = string.Empty;
    public string Officefaxnum { get; set; } = string.Empty;
    public string OldCustomerID { get; set; } = string.Empty;
    public string OldAccountNum { get; set; } = string.Empty;
    public string OldSRN { get; set; } = string.Empty;
    public string OldBranchCode { get; set; } = string.Empty;
    public string? CreditFacilityType { get; set; } = string.Empty;
    public string? PurposeOfFacility { get; set; } = string.Empty;
    public string? FacilityTerm { get; set; } = string.Empty;
    public string DefPaymentStartDate { get; set; } = string.Empty;
    public string? AmountCurrency { get; set; } = string.Empty;
    public string? FacilityAmount { get; set; } = string.Empty;
    public string? DisbursementDate { get; set; } = string.Empty;
    public string? DisbursementAmt { get; set; } = string.Empty;
    public string? MaturityDate { get; set; } = string.Empty;
    public string? SchdInstalAmount { get; set; } = string.Empty;
    public string? RepaymentFreq { get; set; } = string.Empty;
    public string? LastPaymentAmount { get; set; } = string.Empty;
    public string? LastPaymentDate { get; set; } = string.Empty;
    public string? NextPaymentDate { get; set; } = string.Empty;
    public string? CurBal { get; set; } = string.Empty;
    public string? CurBalIndicator { get; set; } = string.Empty;
    public string AssetClassification { get; set; } = string.Empty;
    public string? AmountInArrears { get; set; } = string.Empty;
    public string ArrearsStartDate { get; set; } = string.Empty;
    public string? NDIA { get; set; } = string.Empty;
    public string PaymentHistoryProfile { get; set; } = string.Empty;
    public string AmtOverdue1to30days { get; set; } = string.Empty;
    public string AmtOverdue31to60days { get; set; } = string.Empty;
    public string AmtOverdue61to90days { get; set; } = string.Empty;
    public string AmtOverdue91to120days { get; set; } = string.Empty;
    public string Amtoverdue121To150Days { get; set; } = string.Empty;
    public string AmtOverdue151to180days { get; set; } = string.Empty;
    public string AmtOverdue181orMore { get; set; } = string.Empty;
    public string LegalFlag { get; set; } = string.Empty;
    public string? FacilityStatusCode { get; set; } = string.Empty;
    public string? FacilityStatusDate { get; set; } = string.Empty;
    public string? ClosedDate { get; set; } = string.Empty;
    public string? ClosureReason { get; set; } = string.Empty;
    public string? WrittenOffAmt { get; set; } = string.Empty;
    public string ReasonForWrittenOff { get; set; } = string.Empty;
    public string? DateRestructured { get; set; } = string.Empty;
    public string? ReasonForRestructure { get; set; } = string.Empty;
    public string? CreditCollateralInd { get; set; } = string.Empty;
    public string SecurityType { get; set; } = string.Empty;
    public string NatureOfCharge { get; set; } = string.Empty;
    public string SecurityValue { get; set; } = string.Empty;
    public string CollRegRefNum { get; set; } = string.Empty;
    public string SpecialCommentsCode { get; set; } = string.Empty;
    public string? NatureOfGuarantor1 { get; set; } = string.Empty;
    public string NameOfComGuarantor1 { get; set; } = string.Empty;
    public string BusRegOfGuarantor1 { get; set; } = string.Empty;
    public string G1Surname { get; set; } = string.Empty;
    public string G1FirstName { get; set; } = string.Empty;
    public string G1MiddleNames { get; set; } = string.Empty;
    public string G1NatID { get; set; } = string.Empty;
    public string G1VotID { get; set; } = string.Empty;
    public string G1DrivLic { get; set; } = string.Empty;
    public string G1PassNum { get; set; } = string.Empty;
    public string G1SSN { get; set; } = string.Empty;
    public string G1Gender { get; set; } = string.Empty;
    public string G1DOB { get; set; } = string.Empty;
    public string G1Add1 { get; set; } = string.Empty;
    public string G1Add2 { get; set; } = string.Empty;
    public string G1Add3 { get; set; } = string.Empty;
    public string G1HomeTel { get; set; } = string.Empty;
    public string G1WorkTel { get; set; } = string.Empty;
    public string G1Mobile { get; set; } = string.Empty;
    public string NatureOfGuarantor2 { get; set; } = string.Empty;
    public string NameOfComGuarantor2 { get; set; } = string.Empty;
    public string Busregofguarantor2 { get; set; } = string.Empty;
    public string G2Surname { get; set; } = string.Empty;
    public string G2FirstName { get; set; } = string.Empty;
    public string G2MiddleNames { get; set; } = string.Empty;
    public string G2NatID { get; set; } = string.Empty;
    public string G2VotID { get; set; } = string.Empty;
    public string G2DrivLic { get; set; } = string.Empty;
    public string G2PassNum { get; set; } = string.Empty;
    public string G2SSN { get; set; } = string.Empty;
    public string G2Gender { get; set; } = string.Empty;
    public string G2DOB { get; set; } = string.Empty;
    public string G2Add1 { get; set; } = string.Empty;
    public string G2Add2 { get; set; } = string.Empty;
    public string G2Add3 { get; set; } = string.Empty;
    public string G2HomeTel { get; set; } = string.Empty;
    public string G2WorkTel { get; set; } = string.Empty;
    public string G2Mobile { get; set; } = string.Empty;
    public string NatureOfGuarantor3 { get; set; } = string.Empty;
    public string NameOfComGuarantor3 { get; set; } = string.Empty;
    public string BusRegOfGuarantor3 { get; set; } = string.Empty;
    public string G3Surname { get; set; } = string.Empty;
    public string G3FirstName { get; set; } = string.Empty;
    public string G3MiddleNames { get; set; } = string.Empty;
    public string G3NatID { get; set; } = string.Empty;
    public string G3VotID { get; set; } = string.Empty;
    public string G3DrivLic { get; set; } = string.Empty;
    public string G3PassNum { get; set; } = string.Empty;
    public string G3SSN { get; set; } = string.Empty;
    public string G3Gender { get; set; } = string.Empty;
    public string G3DOB { get; set; } = string.Empty;
    public string G3Add1 { get; set; } = string.Empty;
    public string G3Add2 { get; set; } = string.Empty;
    public string G3Add3 { get; set; } = string.Empty;
    public string G3HomeTel { get; set; } = string.Empty;
    public string G3WorkTel { get; set; } = string.Empty;
    public string G3Mobile { get; set; } = string.Empty;
    public string NatureOfGuarantor4 { get; set; } = string.Empty;
    public string NameOfComGuarantor4 { get; set; } = string.Empty;
    public string BusRegOfGuarantor4 { get; set; } = string.Empty;
    public string G4Surname { get; set; } = string.Empty;
    public string G4FirstName { get; set; } = string.Empty;
    public string G4MiddleNames { get; set; } = string.Empty;
    public string G4NatID { get; set; } = string.Empty;
    public string G4VotID { get; set; } = string.Empty;
    public string G4DrivLic { get; set; } = string.Empty;
    public string G4PassNum { get; set; } = string.Empty;
    public string G4SSN { get; set; } = string.Empty;
    public string G4Gender { get; set; } = string.Empty;
    public string G4DOB { get; set; } = string.Empty;
    public string G4Add1 { get; set; } = string.Empty;
    public string G4Add2 { get; set; } = string.Empty;
    public string G4Add3 { get; set; } = string.Empty;
    public string G4HomeTel { get; set; } = string.Empty;
    public string G4WorkTel { get; set; } = string.Empty;
    public string G4Mobile { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
public class BusinessRef
{
    public int Id { get; set; }
    public int CurrenVersion { get; set; }
    public string SubscriberCode { get; set; }
    public string? CreditFacilityAccNum { get; set; } = string.Empty;
    public string? CustomerID { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string? DisbursementDate { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    // ── Option A: lightweight "when was this last confirmed" tracking ────────
    // NOT a full change-history (that's the external system's job) -- just the
    // most recent reporting period this row was seen/matched again, updated on
    // every match regardless of whether any field actually changed. Answers
    // "as of the last time we processed this subscriber's file, this was on
    // record" for the common case of chasing a recent mismatch.
    public string? LastConfirmedReportingPeriod { get; set; } = string.Empty;
    // ── Identity fields for reference-trust validation (mirrors IndividualRef) ──
    public string? Businessname { get; set; } = string.Empty;
    public string? Busregnum { get; set; } = string.Empty;
    public string? Tinum { get; set; } = string.Empty;
    // ── Closed-facility tracking ────────────────────────────────────────────
    public string? FacilityStatusCode { get; set; } = string.Empty;
}
public class IndividualRef : IIndividualReferenceRow
{
    public int Id { get; set; }
    public int CurrenVersion { get; set; }
    public string SubscriberCode { get; set; }
    public string? CreditFacilityAccNum { get; set; } = string.Empty;
    public string? CustomerID { get; set; } = string.Empty;
    public string? DateOfBirth { get; set; } = string.Empty;
    public string? DisbursementDate { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    // ── Option A: see BusinessRef.LastConfirmedReportingPeriod above ─────────
    public string? LastConfirmedReportingPeriod { get; set; } = string.Empty;
    // ── Sprint 5: Personal ID fields ──────────────────────────────────────────
    public string? NatIDNum     { get; set; } = string.Empty;
    public string? VotersIDNum  { get; set; } = string.Empty;
    public string? DriverLicNum { get; set; } = string.Empty;
    public string? PassportNum  { get; set; } = string.Empty;
    public string? SSNum        { get; set; } = string.Empty;
    public string? EzwichNum    { get; set; } = string.Empty;
    public string? OtherIDNum   { get; set; } = string.Empty;
    // ── Sprint 10: Name fields for reference-trust validation ─────────────────
    public string? Surname    { get; set; } = string.Empty;
    public string? FirstName  { get; set; } = string.Empty;
    public string? MiddleNames { get; set; } = string.Empty;
    // ── Closed-facility tracking ────────────────────────────────────────────
    // Refreshed to the LATEST submitted value every time this facility is
    // seen again (unlike names/IDs, status is a mutable real-world state,
    // not a fact to enrich-once-and-keep).
    public string? FacilityStatusCode { get; set; } = string.Empty;
}
//public class BusinessRef
//{
//    public int Id { get; set; }
//    public int CurrenVersion { get; set; }
//    public string SubscriberCode { get; set; }
//    public string? CreditFacilityAccNum { get; set; } = string.Empty;
//    public string? CustomerID { get; set; } = string.Empty;
//    public string? DisbursementDate { get; set; } = string.Empty;
//    public DateTime CreatedDate { get; set; }
//}
//public class IndividualDUDRef
//{
//    public int Id { get; set; }
//    public int CurrenVersion { get; set; }
//    public string SubscriberCode { get; set; }
//    public string? CreditFacilityAccNum { get; set; } = string.Empty;
//    public string? CustomerID { get; set; } = string.Empty;
//    public string? DateOfBirth { get; set; } = string.Empty;
//    public string? DisbursementDate { get; set; } = string.Empty;
//    public DateTime CreatedDate { get; set; }
//}
public class SettingsClass
{
    public int Id { get; set; }
    public string? Value { get; set; } = string.Empty;
    public SettingsDataType DataType { get; set; }
}
public class BusSettNormalizer
{
    public int Id { get; set; }
    public string? ShortValue { get; set; } = string.Empty;
    public string? LongValue { get; set; } = string.Empty;
    public SettingsDataType DataType { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
}
    public enum SettingsDataType
{
    BusinessName = 0,
    BusinessNamenormalizer
}
