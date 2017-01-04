using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace Parascan0
{
    class HelperClasses
    {
        const string ImagePath = "\\Images\\";

        public static ScreenTip GetNextTip(string applicationPath)
        {
            ScreenTip returnValue = new ScreenTip();
            Random integerRandom = new Random((int)DateTime.Now.Ticks);
            Int32 tipNumber = integerRandom.Next(1, 7);
            switch (tipNumber)
            {
                case 1:
                    returnValue.TextString = "Did you Know?  Dogs and cats become infected with ascarids via ingestion of larvated eggs from a contaminated environment (all ascarid species), ingestion of other vertebrate hosts that have consumed larvated eggs and thus have larvae in their tissues (all species), and ingestion of larvae in the milk of an infected dam (Toxocara spp.).";
                    break;
                case 2:
                    returnValue.TextString = "Did you Know?  Coccidial infections are common in dogs and cats.  Published surveys indicate that coccidia are present in from 3% to 38% of dogs and 3% to 36% of cats in North America. Young animals are more likely than older animals to become infected with coccidia.";
                    break;
                case 3:
                    returnValue.TextString = "Did you Know?  Morulated hookworm eggs are passed in the feces of infected dogs and cats; the most commonly seen species are A. caninum in the dog and A. tubaeforme in the cat.  Adult hookworms in the small intestine of infected dogs or cats are attached to the intestinal villi by a large mouth cavity (buccal cavity).  Male hookworms have a posterior copulatory bursa, and it is not uncommon to observe worms in copula in the small intestine.";
                    break;
                case 4:
                    returnValue.TextString = "Did you Know?  One of the most common causes of a Giardia parasitic infection is the ingestion of infected fecal material, as the cysts are shed in animal feces. The most common cause of transmission is actually waterborne, as the parasite prefers the cool and moist environment. Up to 50 percent of young puppies will develop this intestinal infection, and up to 100 percent of dogs housed in kennels will develop it due to the massive exposure and closely shared living spaces.";
                    break;
                case 5:
                    returnValue.TextString = "Did you Know?  Although normally nonpathogenic in dogs, Eimeria should be considered in any dog which present with diarrhea, where this parasite appears more prevalent. Infections appear to predominate in densely-housed kennels and pet shops, where infections with this parasite and Giardia can become endemic.";
                    break;
                default:
                    returnValue.TextString = "Did you Know?  The genus Trichuris includes over 20 species which infect the large intestine of their host.  A new species - as yet unnamed - has been identified in François’ leaf monkey.  Trichurias is a soil-transmitted helminthias and belongs to the group of neglected tropical diseases, affecting about 604 million people globally.";  
                    break;
            }

            string tipString = Convert.ToString(tipNumber);
            returnValue.ImageString = applicationPath + ImagePath + "Image_Tip_" + Convert.ToString(tipNumber).PadLeft(tipString.Length + 3, '0') + ".PNG";
            return returnValue;
        }

    }

    class ScreenTip
    {
        public string TextString;
        public string ImageString;
    }

    class Specimen
    {
        public string ID;
        public string RequestID;
        public string SpecimenNumber;
        public string ConditionID;
        public string First;
        public string Last;
        public string Nickname;
        public string Species;
        public string Breed;
        public string Gender;
        public string Status;
        public DateTime Date;
        public string Doctor;

        //"Select ac.CaseID, ac.CaseDate as Date, ac.CaseFirst as OwnerFirst, ac.CaseLast as OwnerLast, ac.CaseNickname as PetName, ac.CaseSpecies as Species, ac.CaseDoctor as Doctor, ar.SpecimenNumber as Accession#, ar.RequestID
        public Specimen(string valueID, DateTime valueDate, string valueFirst, string valueLast, string valueNickname, string valueSpecies, string valueDoctor, string valueRequestID)
        {
            ID = valueID;
            RequestID = valueRequestID;
            //SpecimenNumber = valueSpecimenNumber;
            First = valueFirst;
            Last = valueLast;
            Nickname = valueNickname;
            Species = valueSpecies;
            //Breed = valueBreed;
            //Gender = valueGender;
            //Status = valueStatus;
            Date = valueDate;
            Doctor = valueDoctor;
        }

        public Specimen(SqlDataReader objectDataReader)
        {
            // string stringSQL = "Select ac.CaseID, ar.SpecimenNumber as Accession#, ac.CaseTest as Test, ac.CaseFirst as OwnerFirst, ac.CaseLast as OwnerLast, ac.CaseNickname as PetName, ac.CaseSpecies as Species, ac.CaseGender as Sex, ac.CaseStatus as Status, ac.CaseDate as Date From AnalysisCases ac inner join AnalysisRequests ar on ac.CaseID = ar.CaseID Where " + 
            // "ac.CaseID = '" + textCaseID.Text + "' and ac.OrganizationID = '" + Properties.Settings.Default.STRING_ORGANIZATIONID + "' and ac.LocationID = '" + Properties.Settings.Default.STRING_LOCATIONID + "'";

            //Select ac.CaseID, ac.CaseDate as Date, ac.CaseFirst as OwnerFirst, ac.CaseLast as OwnerLast, ac.CaseNickname as PetName, ac.CaseSpecies as Species, ac.CaseDoctor as Doctor, ar.SpecimenNumber as Accession#, ar.RequestID From AnalysisCases ac inner join AnalysisRequests ar on ac.CaseID = ar.CaseID Where "
            ID = Convert.ToString(objectDataReader["CaseID"]);
            RequestID = Convert.ToString(objectDataReader["RequestID"]);
            //SpecimenNumber = Convert.ToString(objectDataReader["Accession#"]);
            //ConditionID = Convert.ToString(objectDataReader["Test"]);
            First = Convert.ToString(objectDataReader["OwnerFirst"]);
            Last = Convert.ToString(objectDataReader["OwnerLast"]);
            Nickname = Convert.ToString(objectDataReader["PetName"]);
            Species = Convert.ToString(objectDataReader["Species"]);
            //Breed = Convert.ToString(objectDataReader["Breed"]);
            //Gender = Convert.ToString(objectDataReader["Sex"]);
            //Status = Convert.ToString(objectDataReader["Status"]);
            Date = Convert.ToDateTime(objectDataReader["Date"]);
            Doctor = Convert.ToString(objectDataReader["Doctor"]);
        }
    }

    
}
