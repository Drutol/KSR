using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR.FuzzySummarization.Model
{
    public class DataRecord
    {
        public int Age { get; set; }
        public Workclass Workclass { get; set; }
        public int SamplingWeight { get; set; }
        public Education Education { get; set; }
        public int EductaionNumber { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public Occupation Occupation { get; set; }
        public Relationship Relationship { get; set; }
        public Race Race { get; set; }
        public bool IsMale { get; set; }
        public int CapitalGain { get; set; }
        public int CapitalLoss { get; set; }
        public int HoursPerWeek { get; set; }
        public NativeCountry NativeCountry { get; set; }
    }

    public enum Workclass
    {
        Private,
        SelpEmployedNotIncorporated,
        SelfEmployedIncorporated,
        FederalGovernor,
        LocalGovernor,
        StateGovernor,
        WithoutPay,
        NeverWorked
    }

    public enum Education
    {
        Bachelor,
        College,
        Grade11,
        HsGraduate,
        ProfSchool,
        AcademicAssociative,
        VocAssociative,
        Grade9,
        Grade7Or8,
        Grade12,
        Masters,
        Grade1To4,
        Grade10,
        Doctorate,
        Grade5To6,
        Preschool
    }

    public enum MaritalStatus
    {
        MarriedCivSpouse,
        Divorced,
        NeverMarried,
        Separated,
        Widowed,
        MarriedSpouseAbsent,
        MarriedAfSpouse
    }

    public enum Occupation
    {
        TechSupport,
        CraftRepair,
        OtherService,
        Sales,
        ExecManagerial,
        ProfSpecialty,
        HandlersCleaners,
        MachineOpInspct,
        AdmClerical,
        FarmingFishing,
        TransportMoving,
        PrivHouseServ,
        ProtectiveServ,
        ArmedForces
    }

    public enum Relationship
    {
        Wife,
        OwnChild,
        Husband,
        NotInFamily,
        OtherRelative,
        Unmarried
    }

    public enum Race
    {
        White,
        AsianPacIslander,
        AmerIndianEskimo,
        Other,
        Black
    }

    public enum NativeCountry
    {
        UnitedSStates,
        Cambodia,
        England,
        PuertoRico,
        Canada,
        Germany,
        OutlyingUs,
        India,
        Japan,
        Greece,
        South,
        China,
        Cuba,
        Iran,
        Honduras,
        Philippines,
        Italy,
        Poland,
        Jamaica,
        Vietnam,
        Mexico,
        Portugal,
        Ireland,
        France,
        DominicanRepublic,
        Laos,
        Ecuador,
        Taiwan,
        Haiti,
        Columbia,
        Hungary,
        Guatemala,
        Nicaragua,
        Scotland,
        Thailand,
        Yugoslavia,
        ElSalvador,
        TrinadadTobago,
        Peru,
        Hong,
        HolandNetherlands
    }
}
