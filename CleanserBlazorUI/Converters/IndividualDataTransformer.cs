namespace CleanserBlazorUI.Converters;
public class IndividualDataTransformer
{
    StringHelper stringHelper = new StringHelper();
    public string UNL_natidnum(string data)
    {
        return data;
    }
    public string UNL_NDIA_OldData(string data)
    {
        return data;
    }
    public string UNL_votersidnum(string data)
    {
        return data;
    }
    public string UNL_driverlicnum(string data)
    {
        return data;
    }
    public string UNL_passportnum(string data)
    {
        return data;
    }
    public string UNL_ssnum(string data)
    {
        return data;
    }
    public string UNL_otheridtype(string data)
    {
        return data;
    }
    public string UNL_otheridnum(string data)
    {
        return data;
    }
    public string UNL_surname(string data)
    {
        return data;
    }
    public string UNL_firstname(string data)
    {
        return data;
    }
    public string UNL_middlenames(string data)
    {
        return data;
    }
    public string UNL_proofofaddtype(string data)
    {
        return data;
    }
    public string UNL_proofofaddnum(string data)
    {
        return data;
    }
    public string UNL_writtenoffamt(string data)
    {
        return data;
    }
    //defective
    public string Data(string data) //1
    {
        return "D";
    }
    public string CorrectionIndicator(string data)
    {
        if (data != "0" && data != "1" && data != "2")
        {
            return "0";
        }
        return data;
    }
    public CellDataAndStatus CreditFacilityAccNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
           data = data.TrimStart().TrimEnd();
        if (data.Length > 0)
        {
            if (data.Contains("+") || stringHelper.HasSixConsecutiveZerosAtEnd(data))
            {
                cellData.Data = data;
                cellData.Passed = false;
                cellData.Errors = new List<string>() { "CREDITFACILITYACCNUM: NOT VALID" };
                return cellData;
            }
            cellData.Data = data;
            cellData.Passed = true;
            return cellData;
        }
        else
        {
            cellData.Errors = new List<string>() { "CREDITFACILITYACCNUM: NOT VALID" };
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }
    }
    public CellDataAndStatus CustomerID(string data)
    {
        var cellData = new CellDataAndStatus(data);
           data = data.TrimStart().TrimEnd();
        if (data.Length > 0)
        {
            if (data.Contains("+") || stringHelper.HasSixConsecutiveZerosAtEnd(data))
            {
                cellData.Data = data;
                cellData.Passed = false;
                cellData.Errors = new List<string>() { "CustomerID: NOT VALID" };
                return cellData;
            }
            cellData.Data = data;
            cellData.Passed = true;
            return cellData;
        }
        else
        {
            cellData.Errors = new List<string>() { "CustomerID: NOT VALID" };
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }

    }

    public CellDataAndStatus BranchCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }

    //Required  Conditional
    public CellDataAndStatus NatIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus VotersIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus DriverLicNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus PassportNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus SSNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus EzwichNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus OtherIDType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus OtherIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus TINum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
        return cellData;
    }
    //[Default: S]
    public CellDataAndStatus MaritalStatus(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormarlizeMaritalStatus(data);
        return cellData;
    }
    public CellDataAndStatus Nationality(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Normalise input before lookup — handles mixed case (e.g. "gha", "Ghana", "GHA" all resolve correctly)
        string normalisedInput = data.Trim().ToUpperInvariant();

        // Keys are always uppercase ISO codes OR common full-name variants subscribers send
        Dictionary<string, string> nationalityCodes = new Dictionary<string, string>
        {
            // ISO2, ISO3, country names, and demonyms.
            {"AD", "ANDORRAN"},
            {"AE", "EMIRATI"},
            {"AF", "AFGHAN"},
            {"AFG", "AFGHAN"},
            {"AFGHAN", "AFGHAN"},
            {"AFGHANISTAN", "AFGHAN"},
            {"AG", "ANTIGUAN"},
            {"AGO", "ANGOLAN"},
            {"AL", "ALBANIAN"},
            {"ALB", "ALBANIAN"},
            {"ALBANIA", "ALBANIAN"},
            {"ALBANIAN", "ALBANIAN"},
            {"ALGERIA", "ALGERIAN"},
            {"ALGERIAN", "ALGERIAN"},
            {"AM", "ARMENIAN"},
            {"AMERICAN", "AMERICAN"},
            {"AMERICAN SAMOA", "AMERICAN SAMOAN"},
            {"AMERICAN SAMOAN", "AMERICAN SAMOAN"},
            {"AND", "ANDORRAN"},
            {"ANDORRA", "ANDORRAN"},
            {"ANDORRAN", "ANDORRAN"},
            {"ANGOLA", "ANGOLAN"},
            {"ANGOLAN", "ANGOLAN"},
            {"ANTARCTIC", "ANTARCTIC"},
            {"ANTARCTICA", "ANTARCTIC"},
            {"ANTIGUA AND BARBUDA", "ANTIGUAN"},
            {"ANTIGUAN", "ANTIGUAN"},
            {"AO", "ANGOLAN"},
            {"AQ", "ANTARCTIC"},
            {"AR", "ARGENTINE"},
            {"ARE", "EMIRATI"},
            {"ARG", "ARGENTINE"},
            {"ARGENTINA", "ARGENTINE"},
            {"ARGENTINE", "ARGENTINE"},
            {"ARM", "ARMENIAN"},
            {"ARMENIA", "ARMENIAN"},
            {"ARMENIAN", "ARMENIAN"},
            {"AS", "AMERICAN SAMOAN"},
            {"ASM", "AMERICAN SAMOAN"},
            {"AT", "AUSTRIAN"},
            {"ATA", "ANTARCTIC"},
            {"ATG", "ANTIGUAN"},
            {"AU", "AUSTRALIAN"},
            {"AUS", "AUSTRALIAN"},
            {"AUSTRALIA", "AUSTRALIAN"},
            {"AUSTRALIAN", "AUSTRALIAN"},
            {"AUSTRIA", "AUSTRIAN"},
            {"AUSTRIAN", "AUSTRIAN"},
            {"AUT", "AUSTRIAN"},
            {"AZ", "AZERBAIJANI"},
            {"AZE", "AZERBAIJANI"},
            {"AZERBAIJAN", "AZERBAIJANI"},
            {"AZERBAIJANI", "AZERBAIJANI"},
            {"BA", "BOSNIAN"},
            {"BAHAMAS", "BAHAMIAN"},
            {"BAHAMIAN", "BAHAMIAN"},
            {"BAHRAIN", "BAHRAINI"},
            {"BAHRAINI", "BAHRAINI"},
            {"BANGLADESH", "BANGLADESHI"},
            {"BANGLADESHI", "BANGLADESHI"},
            {"BARBADIAN", "BARBADIAN"},
            {"BARBADOS", "BARBADIAN"},
            {"BASOTHO", "BASOTHO"},
            {"BB", "BARBADIAN"},
            {"BD", "BANGLADESHI"},
            {"BDI", "BURUNDIAN"},
            {"BE", "BELGIAN"},
            {"BEL", "BELGIAN"},
            {"BELARUS", "BELARUSIAN"},
            {"BELARUSIAN", "BELARUSIAN"},
            {"BELGIAN", "BELGIAN"},
            {"BELGIUM", "BELGIAN"},
            {"BELIZE", "BELIZEAN"},
            {"BELIZEAN", "BELIZEAN"},
            {"BEN", "BENINOIS"},
            {"BENIN", "BENINOIS"},
            {"BENINESE", "BENINOIS"},
            {"BENINOIS", "BENINOIS"},
            {"BF", "BURKINABE"},
            {"BFA", "BURKINABE"},
            {"BG", "BULGARIAN"},
            {"BGD", "BANGLADESHI"},
            {"BGR", "BULGARIAN"},
            {"BH", "BAHRAINI"},
            {"BHR", "BAHRAINI"},
            {"BHS", "BAHAMIAN"},
            {"BHUTAN", "BHUTANESE"},
            {"BHUTANESE", "BHUTANESE"},
            {"BI", "BURUNDIAN"},
            {"BIH", "BOSNIAN"},
            {"BISSAU GUINEAN", "BISSAU-GUINEAN"},
            {"BISSAU-GUINEAN", "BISSAU-GUINEAN"},
            {"BISSAUGUINEAN", "BISSAU-GUINEAN"},
            {"BJ", "BENINOIS"},
            {"BLR", "BELARUSIAN"},
            {"BLZ", "BELIZEAN"},
            {"BN", "BRUNEIAN"},
            {"BO", "BOLIVIAN"},
            {"BOL", "BOLIVIAN"},
            {"BOLIVIA", "BOLIVIAN"},
            {"BOLIVIAN", "BOLIVIAN"},
            {"BOSNIA AND HERZEGOVINA", "BOSNIAN"},
            {"BOSNIAN", "BOSNIAN"},
            {"BOTSWANA", "BOTSWANAN"},
            {"BOTSWANAN", "BOTSWANAN"},
            {"BR", "BRAZILIAN"},
            {"BRA", "BRAZILIAN"},
            {"BRAZIL", "BRAZILIAN"},
            {"BRAZILIAN", "BRAZILIAN"},
            {"BRB", "BARBADIAN"},
            {"BRITISH", "BRITISH"},
            {"BRN", "BRUNEIAN"},
            {"BRUNEI", "BRUNEIAN"},
            {"BRUNEIAN", "BRUNEIAN"},
            {"BS", "BAHAMIAN"},
            {"BT", "BHUTANESE"},
            {"BTN", "BHUTANESE"},
            {"BULGARIA", "BULGARIAN"},
            {"BULGARIAN", "BULGARIAN"},
            {"BURKINA FASO", "BURKINABE"},
            {"BURKINABE", "BURKINABE"},
            {"BURMESE", "BURMESE"},
            {"BURUNDI", "BURUNDIAN"},
            {"BURUNDIAN", "BURUNDIAN"},
            {"BW", "BOTSWANAN"},
            {"BWA", "BOTSWANAN"},
            {"BY", "BELARUSIAN"},
            {"BZ", "BELIZEAN"},
            {"CA", "CANADIAN"},
            {"CABO VERDE", "CAPE VERDEAN"},
            {"CAF", "CENTRAL AFRICAN"},
            {"CAMBODIA", "CAMBODIAN"},
            {"CAMBODIAN", "CAMBODIAN"},
            {"CAMEROON", "CAMEROONIAN"},
            {"CAMEROONIAN", "CAMEROONIAN"},
            {"CAN", "CANADIAN"},
            {"CANADA", "CANADIAN"},
            {"CANADIAN", "CANADIAN"},
            {"CAPE VERDEAN", "CAPE VERDEAN"},
            {"CD", "CONGOLESE"},
            {"CENTRAL AFRICAN", "CENTRAL AFRICAN"},
            {"CENTRAL AFRICAN REPUBLIC", "CENTRAL AFRICAN"},
            {"CF", "CENTRAL AFRICAN"},
            {"CG", "CONGOLESE"},
            {"CH", "SWISS"},
            {"CHAD", "CHADIAN"},
            {"CHADIAN", "CHADIAN"},
            {"CHE", "SWISS"},
            {"CHILE", "CHILEAN"},
            {"CHILEAN", "CHILEAN"},
            {"CHINA", "CHINESE"},
            {"CHINESE", "CHINESE"},
            {"CHL", "CHILEAN"},
            {"CHN", "CHINESE"},
            {"CI", "IVORIAN"},
            {"CIV", "IVORIAN"},
            {"CL", "CHILEAN"},
            {"CM", "CAMEROONIAN"},
            {"CMR", "CAMEROONIAN"},
            {"CN", "CHINESE"},
            {"CO", "COLOMBIAN"},
            {"COD", "CONGOLESE"},
            {"COG", "CONGOLESE"},
            {"COL", "COLOMBIAN"},
            {"COLOMBIA", "COLOMBIAN"},
            {"COLOMBIAN", "COLOMBIAN"},
            {"COM", "COMORIAN"},
            {"COMORIAN", "COMORIAN"},
            {"COMOROS", "COMORIAN"},
            {"CONGO", "CONGOLESE"},
            {"CONGOLESE", "CONGOLESE"},
            {"COSTA RICA", "COSTA RICAN"},
            {"COSTA RICAN", "COSTA RICAN"},
            {"COTE D IVOIRE", "IVORIAN"},
            {"COTE D'IVOIRE", "IVORIAN"},
            {"COTE DIVOIRE", "IVORIAN"},
            {"CPV", "CAPE VERDEAN"},
            {"CR", "COSTA RICAN"},
            {"CRI", "COSTA RICAN"},
            {"CROATIA", "CROATIAN"},
            {"CROATIAN", "CROATIAN"},
            {"CU", "CUBAN"},
            {"CUB", "CUBAN"},
            {"CUBA", "CUBAN"},
            {"CUBAN", "CUBAN"},
            {"CV", "CAPE VERDEAN"},
            {"CY", "CYPRIOT"},
            {"CYP", "CYPRIOT"},
            {"CYPRIOT", "CYPRIOT"},
            {"CYPRUS", "CYPRIOT"},
            {"CZ", "CZECH"},
            {"CZE", "CZECH"},
            {"CZECH", "CZECH"},
            {"CZECH REPUBLIC", "CZECH"},
            {"DANISH", "DANISH"},
            {"DE", "GERMAN"},
            {"DEMOCRATIC REPUBLIC OF THE CONGO", "CONGOLESE"},
            {"DENMARK", "DANISH"},
            {"DEU", "GERMAN"},
            {"DJ", "DJIBOUTIAN"},
            {"DJI", "DJIBOUTIAN"},
            {"DJIBOUTI", "DJIBOUTIAN"},
            {"DJIBOUTIAN", "DJIBOUTIAN"},
            {"DK", "DANISH"},
            {"DM", "DOMINICAN"},
            {"DMA", "DOMINICAN"},
            {"DNK", "DANISH"},
            {"DO", "DOMINICAN"},
            {"DOM", "DOMINICAN"},
            {"DOMINICA", "DOMINICAN"},
            {"DOMINICAN", "DOMINICAN"},
            {"DOMINICAN REPUBLIC", "DOMINICAN"},
            {"DUTCH", "DUTCH"},
            {"DZ", "ALGERIAN"},
            {"DZA", "ALGERIAN"},
            {"EC", "ECUADORIAN"},
            {"ECU", "ECUADORIAN"},
            {"ECUADOR", "ECUADORIAN"},
            {"ECUADORIAN", "ECUADORIAN"},
            {"EE", "ESTONIAN"},
            {"EG", "EGYPTIAN"},
            {"EGY", "EGYPTIAN"},
            {"EGYPT", "EGYPTIAN"},
            {"EGYPTIAN", "EGYPTIAN"},
            {"EL SALVADOR", "SALVADORAN"},
            {"EMIRATI", "EMIRATI"},
            {"ENGLAND", "BRITISH"},
            {"EQUATOGUINEAN", "EQUATOGUINEAN"},
            {"EQUATORIAL GUINEA", "EQUATOGUINEAN"},
            {"ER", "ERITREAN"},
            {"ERI", "ERITREAN"},
            {"ERITREA", "ERITREAN"},
            {"ERITREAN", "ERITREAN"},
            {"ES", "SPANISH"},
            {"ESP", "SPANISH"},
            {"EST", "ESTONIAN"},
            {"ESTONIA", "ESTONIAN"},
            {"ESTONIAN", "ESTONIAN"},
            {"ESWATINI", "SWAZI"},
            {"ET", "ETHIOPIAN"},
            {"ETH", "ETHIOPIAN"},
            {"ETHIOPIA", "ETHIOPIAN"},
            {"ETHIOPIAN", "ETHIOPIAN"},
            {"EUR", "EUROPEAN"},
            {"EUROPEAN", "EUROPEAN"},
            {"FI", "FINNISH"},
            {"FIJI", "FIJIAN"},
            {"FIJIAN", "FIJIAN"},
            {"FILIPINO", "FILIPINO"},
            {"FIN", "FINNISH"},
            {"FINLAND", "FINNISH"},
            {"FINNISH", "FINNISH"},
            {"FJ", "FIJIAN"},
            {"FJI", "FIJIAN"},
            {"FM", "MICRONESIAN"},
            {"FR", "FRENCH"},
            {"FRA", "FRENCH"},
            {"FRANCE", "FRENCH"},
            {"FRENCH", "FRENCH"},
            {"FRENCH GUIANA", "FRENCH GUIANESE"},
            {"FRENCH GUIANESE", "FRENCH GUIANESE"},
            {"FSM", "MICRONESIAN"},
            {"GA", "GABONESE"},
            {"GAB", "GABONESE"},
            {"GABON", "GABONESE"},
            {"GABONESE", "GABONESE"},
            {"GAMBIA", "GAMBIAN"},
            {"GAMBIAN", "GAMBIAN"},
            {"GB", "BRITISH"},
            {"GBR", "BRITISH"},
            {"GD", "GRENADIAN"},
            {"GE", "GEORGIAN"},
            {"GEO", "GEORGIAN"},
            {"GEORGIA", "GEORGIAN"},
            {"GEORGIAN", "GEORGIAN"},
            {"GERMAN", "GERMAN"},
            {"GERMANY", "GERMAN"},
            {"GF", "FRENCH GUIANESE"},
            {"GG", "GUERN"},
            {"GGY", "GUERN"},
            {"GH", "GHANAIAN"},
            {"GHA", "GHANAIAN"},
            {"GHANA", "GHANAIAN"},
            {"GHANAIAN", "GHANAIAN"},
            {"GHS", "GHANAIAN"},
            {"GIN", "GUINEAN"},
            {"GM", "GAMBIAN"},
            {"GMB", "GAMBIAN"},
            {"GN", "GUINEAN"},
            {"GNB", "BISSAU-GUINEAN"},
            {"GNQ", "EQUATOGUINEAN"},
            {"GQ", "EQUATOGUINEAN"},
            {"GR", "GREEK"},
            {"GRC", "GREEK"},
            {"GRD", "GRENADIAN"},
            {"GREECE", "GREEK"},
            {"GREEK", "GREEK"},
            {"GRENADA", "GRENADIAN"},
            {"GRENADIAN", "GRENADIAN"},
            {"GT", "GUATEMALAN"},
            {"GTM", "GUATEMALAN"},
            {"GUATEMALA", "GUATEMALAN"},
            {"GUATEMALAN", "GUATEMALAN"},
            {"GUERN", "GUERN"},
            {"GUERNSEY", "GUERN"},
            {"GUF", "FRENCH GUIANESE"},
            {"GUINEA", "GUINEAN"},
            {"GUINEA BISSAU", "BISSAU-GUINEAN"},
            {"GUINEA-BISSAU", "BISSAU-GUINEAN"},
            {"GUINEABISSAU", "BISSAU-GUINEAN"},
            {"GUINEAN", "GUINEAN"},
            {"GUY", "GUYANESE"},
            {"GUYANA", "GUYANESE"},
            {"GUYANESE", "GUYANESE"},
            {"GW", "BISSAU-GUINEAN"},
            {"GY", "GUYANESE"},
            {"HAITI", "HAITIAN"},
            {"HAITIAN", "HAITIAN"},
            {"HN", "HONDURAN"},
            {"HND", "HONDURAN"},
            {"HOLLAND", "DUTCH"},
            {"HONDURAN", "HONDURAN"},
            {"HONDURAS", "HONDURAN"},
            {"HR", "CROATIAN"},
            {"HRV", "CROATIAN"},
            {"HT", "HAITIAN"},
            {"HTI", "HAITIAN"},
            {"HU", "HUNGARIAN"},
            {"HUN", "HUNGARIAN"},
            {"HUNGARIAN", "HUNGARIAN"},
            {"HUNGARY", "HUNGARIAN"},
            {"I KIRIBATI", "I-KIRIBATI"},
            {"I-KIRIBATI", "I-KIRIBATI"},
            {"ICELAND", "ICELANDER"},
            {"ICELANDER", "ICELANDER"},
            {"ID", "INDONESIAN"},
            {"IDN", "INDONESIAN"},
            {"IE", "IRISH"},
            {"IKIRIBATI", "I-KIRIBATI"},
            {"IL", "ISRAELI"},
            {"IN", "INDIAN"},
            {"IND", "INDIAN"},
            {"INDIA", "INDIAN"},
            {"INDIAN", "INDIAN"},
            {"INDONESIA", "INDONESIAN"},
            {"INDONESIAN", "INDONESIAN"},
            {"IQ", "IRAQI"},
            {"IR", "IRANIAN"},
            {"IRAN", "IRANIAN"},
            {"IRANIAN", "IRANIAN"},
            {"IRAQ", "IRAQI"},
            {"IRAQI", "IRAQI"},
            {"IRELAND", "IRISH"},
            {"IRISH", "IRISH"},
            {"IRL", "IRISH"},
            {"IRN", "IRANIAN"},
            {"IRQ", "IRAQI"},
            {"IS", "ICELANDER"},
            {"ISL", "ICELANDER"},
            {"ISR", "ISRAELI"},
            {"ISRAEL", "ISRAELI"},
            {"ISRAELI", "ISRAELI"},
            {"IT", "ITALIAN"},
            {"ITA", "ITALIAN"},
            {"ITALIAN", "ITALIAN"},
            {"ITALY", "ITALIAN"},
            {"IVORIAN", "IVORIAN"},
            {"IVORY COAST", "IVORIAN"},
            {"JAM", "JAMAICAN"},
            {"JAMAICA", "JAMAICAN"},
            {"JAMAICAN", "JAMAICAN"},
            {"JAPAN", "JAPANESE"},
            {"JAPANESE", "JAPANESE"},
            {"JM", "JAMAICAN"},
            {"JO", "JORDANIAN"},
            {"JOR", "JORDANIAN"},
            {"JORDAN", "JORDANIAN"},
            {"JORDANIAN", "JORDANIAN"},
            {"JP", "JAPANESE"},
            {"JPN", "JAPANESE"},
            {"KAZ", "KAZAKH"},
            {"KAZAKH", "KAZAKH"},
            {"KAZAKHSTAN", "KAZAKH"},
            {"KE", "KENYAN"},
            {"KEN", "KENYAN"},
            {"KENYA", "KENYAN"},
            {"KENYAN", "KENYAN"},
            {"KG", "KYRGYZ"},
            {"KGZ", "KYRGYZ"},
            {"KH", "CAMBODIAN"},
            {"KHM", "CAMBODIAN"},
            {"KI", "I-KIRIBATI"},
            {"KIR", "I-KIRIBATI"},
            {"KIRIBATI", "I-KIRIBATI"},
            {"KITTITIAN", "KITTITIAN"},
            {"KM", "COMORIAN"},
            {"KN", "KITTITIAN"},
            {"KNA", "KITTITIAN"},
            {"KOR", "SOUTH KOREAN"},
            {"KOREA", "SOUTH KOREAN"},
            {"KP", "NORTH KOREAN"},
            {"KR", "SOUTH KOREAN"},
            {"KUWAIT", "KUWAITI"},
            {"KUWAITI", "KUWAITI"},
            {"KW", "KUWAITI"},
            {"KWT", "KUWAITI"},
            {"KYRGYZ", "KYRGYZ"},
            {"KYRGYZSTAN", "KYRGYZ"},
            {"KZ", "KAZAKH"},
            {"LA", "LAO"},
            {"LAO", "LAO"},
            {"LAOS", "LAO"},
            {"LATVIA", "LATVIAN"},
            {"LATVIAN", "LATVIAN"},
            {"LB", "LEBANESE"},
            {"LBN", "LEBANESE"},
            {"LBR", "LIBERIAN"},
            {"LBY", "LIBYAN"},
            {"LC", "SAINT LUCIAN"},
            {"LCA", "SAINT LUCIAN"},
            {"LEBANESE", "LEBANESE"},
            {"LEBANON", "LEBANESE"},
            {"LESOTHO", "BASOTHO"},
            {"LI", "LIECHTENSTEINER"},
            {"LIBERIA", "LIBERIAN"},
            {"LIBERIAN", "LIBERIAN"},
            {"LIBYA", "LIBYAN"},
            {"LIBYAN", "LIBYAN"},
            {"LIE", "LIECHTENSTEINER"},
            {"LIECHTENSTEIN", "LIECHTENSTEINER"},
            {"LIECHTENSTEINER", "LIECHTENSTEINER"},
            {"LITHUANIA", "LITHUANIAN"},
            {"LITHUANIAN", "LITHUANIAN"},
            {"LK", "SRI LANKAN"},
            {"LKA", "SRI LANKAN"},
            {"LR", "LIBERIAN"},
            {"LS", "BASOTHO"},
            {"LSO", "BASOTHO"},
            {"LT", "LITHUANIAN"},
            {"LTU", "LITHUANIAN"},
            {"LU", "LUXEMBOURGER"},
            {"LUX", "LUXEMBOURGER"},
            {"LUXEMBOURG", "LUXEMBOURGER"},
            {"LUXEMBOURGER", "LUXEMBOURGER"},
            {"LV", "LATVIAN"},
            {"LVA", "LATVIAN"},
            {"LY", "LIBYAN"},
            {"MA", "MOROCCAN"},
            {"MAC", "MACANESE"},
            {"MACANESE", "MACANESE"},
            {"MACAO", "MACANESE"},
            {"MACEDONIAN", "MACEDONIAN"},
            {"MADAGASCAR", "MALAGASY"},
            {"MALAGASY", "MALAGASY"},
            {"MALAWI", "MALAWIAN"},
            {"MALAWIAN", "MALAWIAN"},
            {"MALAYSIA", "MALAYSIAN"},
            {"MALAYSIAN", "MALAYSIAN"},
            {"MALDIVES", "MALDIVIAN"},
            {"MALDIVIAN", "MALDIVIAN"},
            {"MALI", "MALIAN"},
            {"MALIAN", "MALIAN"},
            {"MALTA", "MALTESE"},
            {"MALTESE", "MALTESE"},
            {"MAR", "MOROCCAN"},
            {"MARSHALL ISLANDS", "MARSHALLESE"},
            {"MARSHALLESE", "MARSHALLESE"},
            {"MAURITANIA", "MAURITANIAN"},
            {"MAURITANIAN", "MAURITANIAN"},
            {"MAURITIAN", "MAURITIAN"},
            {"MAURITIUS", "MAURITIAN"},
            {"MC", "MONEGASQUE"},
            {"MCO", "MONEGASQUE"},
            {"MD", "MOLDOVAN"},
            {"MDA", "MOLDOVAN"},
            {"MDG", "MALAGASY"},
            {"MDV", "MALDIVIAN"},
            {"ME", "MONTENEGRIN"},
            {"MEX", "MEXICAN"},
            {"MEXICAN", "MEXICAN"},
            {"MEXICO", "MEXICAN"},
            {"MG", "MALAGASY"},
            {"MH", "MARSHALLESE"},
            {"MHL", "MARSHALLESE"},
            {"MICRONESIA", "MICRONESIAN"},
            {"MICRONESIAN", "MICRONESIAN"},
            {"MK", "MACEDONIAN"},
            {"MKD", "MACEDONIAN"},
            {"ML", "MALIAN"},
            {"MLI", "MALIAN"},
            {"MLT", "MALTESE"},
            {"MM", "BURMESE"},
            {"MMR", "BURMESE"},
            {"MN", "MONGOLIAN"},
            {"MNE", "MONTENEGRIN"},
            {"MNG", "MONGOLIAN"},
            {"MO", "MACANESE"},
            {"MOLDOVA", "MOLDOVAN"},
            {"MOLDOVAN", "MOLDOVAN"},
            {"MONACO", "MONEGASQUE"},
            {"MONEGASQUE", "MONEGASQUE"},
            {"MONGOLIA", "MONGOLIAN"},
            {"MONGOLIAN", "MONGOLIAN"},
            {"MONTENEGRIN", "MONTENEGRIN"},
            {"MONTENEGRO", "MONTENEGRIN"},
            {"MOROCCAN", "MOROCCAN"},
            {"MOROCCO", "MOROCCAN"},
            {"MOZ", "MOZAMBICAN"},
            {"MOZAMBICAN", "MOZAMBICAN"},
            {"MOZAMBIQUE", "MOZAMBICAN"},
            {"MR", "MAURITANIAN"},
            {"MRT", "MAURITANIAN"},
            {"MT", "MALTESE"},
            {"MU", "MAURITIAN"},
            {"MUS", "MAURITIAN"},
            {"MV", "MALDIVIAN"},
            {"MW", "MALAWIAN"},
            {"MWI", "MALAWIAN"},
            {"MX", "MEXICAN"},
            {"MY", "MALAYSIAN"},
            {"MYANMAR", "BURMESE"},
            {"MYS", "MALAYSIAN"},
            {"MZ", "MOZAMBICAN"},
            {"NA", "NAMIBIAN"},
            {"NAM", "NAMIBIAN"},
            {"NAMIBIA", "NAMIBIAN"},
            {"NAMIBIAN", "NAMIBIAN"},
            {"NAURU", "NAURUAN"},
            {"NAURUAN", "NAURUAN"},
            {"NE", "NIGERIEN"},
            {"NEPAL", "NEPALI"},
            {"NEPALI", "NEPALI"},
            {"NER", "NIGERIEN"},
            {"NETHERLANDS", "DUTCH"},
            {"NEW ZEALAND", "NEW ZEALANDER"},
            {"NEW ZEALANDER", "NEW ZEALANDER"},
            {"NG", "NIGERIAN"},
            {"NGA", "NIGERIAN"},
            {"NI", "NICARAGUAN"},
            {"NI VANUATU", "NI-VANUATU"},
            {"NI-VANUATU", "NI-VANUATU"},
            {"NIC", "NICARAGUAN"},
            {"NICARAGUA", "NICARAGUAN"},
            {"NICARAGUAN", "NICARAGUAN"},
            {"NIG", "NIGERIAN"},
            {"NIGER", "NIGERIEN"},
            {"NIGERIA", "NIGERIAN"},
            {"NIGERIAN", "NIGERIAN"},
            {"NIGERIEN", "NIGERIEN"},
            {"NIVANUATU", "NI-VANUATU"},
            {"NL", "DUTCH"},
            {"NLD", "DUTCH"},
            {"NO", "NORWEGIAN"},
            {"NOR", "NORWEGIAN"},
            {"NORTH KOREA", "NORTH KOREAN"},
            {"NORTH KOREAN", "NORTH KOREAN"},
            {"NORTH MACEDONIA", "MACEDONIAN"},
            {"NORWAY", "NORWEGIAN"},
            {"NORWEGIAN", "NORWEGIAN"},
            {"NP", "NEPALI"},
            {"NPL", "NEPALI"},
            {"NR", "NAURUAN"},
            {"NRU", "NAURUAN"},
            {"NZ", "NEW ZEALANDER"},
            {"NZL", "NEW ZEALANDER"},
            {"OM", "OMANI"},
            {"OMAN", "OMANI"},
            {"OMANI", "OMANI"},
            {"OMN", "OMANI"},
            {"PA", "PANAMANIAN"},
            {"PAK", "PAKISTANI"},
            {"PAKISTAN", "PAKISTANI"},
            {"PAKISTANI", "PAKISTANI"},
            {"PALAU", "PALAUAN"},
            {"PALAUAN", "PALAUAN"},
            {"PALESTINE", "PALESTINIAN"},
            {"PALESTINIAN", "PALESTINIAN"},
            {"PAN", "PANAMANIAN"},
            {"PANAMA", "PANAMANIAN"},
            {"PANAMANIAN", "PANAMANIAN"},
            {"PAPUA NEW GUINEA", "PAPUA NEW GUINEAN"},
            {"PAPUA NEW GUINEAN", "PAPUA NEW GUINEAN"},
            {"PARAGUAY", "PARAGUAYAN"},
            {"PARAGUAYAN", "PARAGUAYAN"},
            {"PE", "PERUVIAN"},
            {"PER", "PERUVIAN"},
            {"PERU", "PERUVIAN"},
            {"PERUVIAN", "PERUVIAN"},
            {"PG", "PAPUA NEW GUINEAN"},
            {"PH", "FILIPINO"},
            {"PHILIPPINES", "FILIPINO"},
            {"PHL", "FILIPINO"},
            {"PK", "PAKISTANI"},
            {"PL", "POLISH"},
            {"PLW", "PALAUAN"},
            {"PNG", "PAPUA NEW GUINEAN"},
            {"POL", "POLISH"},
            {"POLAND", "POLISH"},
            {"POLISH", "POLISH"},
            {"PORTUGAL", "PORTUGUESE"},
            {"PORTUGUESE", "PORTUGUESE"},
            {"PRK", "NORTH KOREAN"},
            {"PRT", "PORTUGUESE"},
            {"PRY", "PARAGUAYAN"},
            {"PS", "PALESTINIAN"},
            {"PSE", "PALESTINIAN"},
            {"PT", "PORTUGUESE"},
            {"PW", "PALAUAN"},
            {"PY", "PARAGUAYAN"},
            {"QA", "QATARI"},
            {"QAT", "QATARI"},
            {"QATAR", "QATARI"},
            {"QATARI", "QATARI"},
            {"RO", "ROMANIAN"},
            {"ROMANIA", "ROMANIAN"},
            {"ROMANIAN", "ROMANIAN"},
            {"ROU", "ROMANIAN"},
            {"RS", "SERBIAN"},
            {"RU", "RUSSIAN"},
            {"RUS", "RUSSIAN"},
            {"RUSSIA", "RUSSIAN"},
            {"RUSSIAN", "RUSSIAN"},
            {"RW", "RWANDAN"},
            {"RWA", "RWANDAN"},
            {"RWANDA", "RWANDAN"},
            {"RWANDAN", "RWANDAN"},
            {"SA", "SAUDI"},
            {"SAINT KITTS AND NEVIS", "KITTITIAN"},
            {"SAINT LUCIA", "SAINT LUCIAN"},
            {"SAINT LUCIAN", "SAINT LUCIAN"},
            {"SAINT VINCENT AND THE GRENADINES", "VINCENTIAN"},
            {"SALVADORAN", "SALVADORAN"},
            {"SAMMARINESE", "SAMMARINESE"},
            {"SAMOA", "SAMOAN"},
            {"SAMOAN", "SAMOAN"},
            {"SAN MARINO", "SAMMARINESE"},
            {"SAO TOME AND PRINCIPE", "SAO TOMEAN"},
            {"SAO TOMEAN", "SAO TOMEAN"},
            {"SAU", "SAUDI"},
            {"SAUDI", "SAUDI"},
            {"SAUDI ARABIA", "SAUDI"},
            {"SB", "SOLOMON ISLANDER"},
            {"SC", "SEYCHELLOIS"},
            {"SCOTLAND", "BRITISH"},
            {"SD", "SUDANESE"},
            {"SDN", "SUDANESE"},
            {"SE", "SWEDISH"},
            {"SEN", "SENEGALESE"},
            {"SENEGAL", "SENEGALESE"},
            {"SENEGALESE", "SENEGALESE"},
            {"SERBIA", "SERBIAN"},
            {"SERBIAN", "SERBIAN"},
            {"SEYCHELLES", "SEYCHELLOIS"},
            {"SEYCHELLOIS", "SEYCHELLOIS"},
            {"SG", "SINGAPOREAN"},
            {"SGP", "SINGAPOREAN"},
            {"SI", "SLOVENE"},
            {"SIERRA LEONE", "SIERRA LEONEAN"},
            {"SIERRA LEONEAN", "SIERRA LEONEAN"},
            {"SIERRALEONEAN", "SIERRA LEONEAN"},
            {"SINGAPORE", "SINGAPOREAN"},
            {"SINGAPOREAN", "SINGAPOREAN"},
            {"SK", "SLOVAK"},
            {"SL", "SIERRA LEONEAN"},
            {"SLA", "SIERRA LEONEAN"},
            {"SLB", "SOLOMON ISLANDER"},
            {"SLE", "SIERRA LEONEAN"},
            {"SLOVAK", "SLOVAK"},
            {"SLOVAKIA", "SLOVAK"},
            {"SLOVENE", "SLOVENE"},
            {"SLOVENIA", "SLOVENE"},
            {"SLV", "SALVADORAN"},
            {"SM", "SAMMARINESE"},
            {"SMR", "SAMMARINESE"},
            {"SN", "SENEGALESE"},
            {"SO", "SOMALI"},
            {"SOLOMON ISLANDER", "SOLOMON ISLANDER"},
            {"SOLOMON ISLANDS", "SOLOMON ISLANDER"},
            {"SOM", "SOMALI"},
            {"SOMALI", "SOMALI"},
            {"SOMALIA", "SOMALI"},
            {"SOUTH AFRICA", "SOUTH AFRICAN"},
            {"SOUTH AFRICAN", "SOUTH AFRICAN"},
            {"SOUTH KOREA", "SOUTH KOREAN"},
            {"SOUTH KOREAN", "SOUTH KOREAN"},
            {"SOUTH SUDAN", "SOUTH SUDANESE"},
            {"SOUTH SUDANESE", "SOUTH SUDANESE"},
            {"SPAIN", "SPANISH"},
            {"SPANISH", "SPANISH"},
            {"SR", "SURINAMESE"},
            {"SRB", "SERBIAN"},
            {"SRI LANKA", "SRI LANKAN"},
            {"SRI LANKAN", "SRI LANKAN"},
            {"SRILANKAN", "SRI LANKAN"},
            {"SS", "SOUTH SUDANESE"},
            {"SSD", "SOUTH SUDANESE"},
            {"ST", "SAO TOMEAN"},
            {"STP", "SAO TOMEAN"},
            {"SUDAN", "SUDANESE"},
            {"SUDANESE", "SUDANESE"},
            {"SUR", "SURINAMESE"},
            {"SURINAME", "SURINAMESE"},
            {"SURINAMESE", "SURINAMESE"},
            {"SV", "SALVADORAN"},
            {"SVK", "SLOVAK"},
            {"SVN", "SLOVENE"},
            {"SWAZI", "SWAZI"},
            {"SWE", "SWEDISH"},
            {"SWEDEN", "SWEDISH"},
            {"SWEDISH", "SWEDISH"},
            {"SWISS", "SWISS"},
            {"SWITZERLAND", "SWISS"},
            {"SWZ", "SWAZI"},
            {"SY", "SYRIAN"},
            {"SYC", "SEYCHELLOIS"},
            {"SYR", "SYRIAN"},
            {"SYRIA", "SYRIAN"},
            {"SYRIAN", "SYRIAN"},
            {"SZ", "SWAZI"},
            {"TAIWAN", "TAIWANESE"},
            {"TAIWANESE", "TAIWANESE"},
            {"TAJIK", "TAJIK"},
            {"TAJIKISTAN", "TAJIK"},
            {"TANZANIA", "TANZANIAN"},
            {"TANZANIAN", "TANZANIAN"},
            {"TCD", "CHADIAN"},
            {"TD", "CHADIAN"},
            {"TG", "TOGOLESE"},
            {"TGO", "TOGOLESE"},
            {"TH", "THAI"},
            {"THA", "THAI"},
            {"THAI", "THAI"},
            {"THAILAND", "THAI"},
            {"TIMOR LESTE", "TIMORESE"},
            {"TIMOR-LESTE", "TIMORESE"},
            {"TIMORESE", "TIMORESE"},
            {"TIMORLESTE", "TIMORESE"},
            {"TJ", "TAJIK"},
            {"TJK", "TAJIK"},
            {"TKM", "TURKMEN"},
            {"TL", "TIMORESE"},
            {"TLS", "TIMORESE"},
            {"TM", "TURKMEN"},
            {"TN", "TUNISIAN"},
            {"TO", "TONGAN"},
            {"TOG", "TOGOLESE"},
            {"TOGO", "TOGOLESE"},
            {"TOGOLESE", "TOGOLESE"},
            {"TON", "TONGAN"},
            {"TONGA", "TONGAN"},
            {"TONGAN", "TONGAN"},
            {"TR", "TURKISH"},
            {"TRINIDAD AND TOBAGO", "TRINIDADIAN"},
            {"TRINIDADIAN", "TRINIDADIAN"},
            {"TT", "TRINIDADIAN"},
            {"TTO", "TRINIDADIAN"},
            {"TUN", "TUNISIAN"},
            {"TUNISIA", "TUNISIAN"},
            {"TUNISIAN", "TUNISIAN"},
            {"TUR", "TURKISH"},
            {"TURKEY", "TURKISH"},
            {"TURKISH", "TURKISH"},
            {"TURKMEN", "TURKMEN"},
            {"TURKMENISTAN", "TURKMEN"},
            {"TUV", "TUVALUAN"},
            {"TUVALU", "TUVALUAN"},
            {"TUVALUAN", "TUVALUAN"},
            {"TV", "TUVALUAN"},
            {"TW", "TAIWANESE"},
            {"TWN", "TAIWANESE"},
            {"TZ", "TANZANIAN"},
            {"TZA", "TANZANIAN"},
            {"UA", "UKRAINIAN"},
            {"UAE", "EMIRATI"},
            {"UG", "UGANDAN"},
            {"UGA", "UGANDAN"},
            {"UGANDA", "UGANDAN"},
            {"UGANDAN", "UGANDAN"},
            {"UK", "BRITISH"},
            {"UKR", "UKRAINIAN"},
            {"UKRAINE", "UKRAINIAN"},
            {"UKRAINIAN", "UKRAINIAN"},
            {"UNITED ARAB EMIRATES", "EMIRATI"},
            {"UNITED KINGDOM", "BRITISH"},
            {"UNITED STATES", "AMERICAN"},
            {"URUGUAY", "URUGUAYAN"},
            {"URUGUAYAN", "URUGUAYAN"},
            {"URY", "URUGUAYAN"},
            {"US", "AMERICAN"},
            {"USA", "AMERICAN"},
            {"UY", "URUGUAYAN"},
            {"UZ", "UZBEK"},
            {"UZB", "UZBEK"},
            {"UZBEK", "UZBEK"},
            {"UZBEKISTAN", "UZBEK"},
            {"VA", "VATICAN"},
            {"VANUATU", "NI-VANUATU"},
            {"VAT", "VATICAN"},
            {"VATICAN", "VATICAN"},
            {"VATICAN CITY", "VATICAN"},
            {"VC", "VINCENTIAN"},
            {"VCT", "VINCENTIAN"},
            {"VE", "VENEZUELAN"},
            {"VEN", "VENEZUELAN"},
            {"VENEZUELA", "VENEZUELAN"},
            {"VENEZUELAN", "VENEZUELAN"},
            {"VIETNAM", "VIETNAMESE"},
            {"VIETNAMESE", "VIETNAMESE"},
            {"VINCENTIAN", "VINCENTIAN"},
            {"VN", "VIETNAMESE"},
            {"VNM", "VIETNAMESE"},
            {"VU", "NI-VANUATU"},
            {"VUT", "NI-VANUATU"},
            {"WALES", "BRITISH"},
            {"WEST AFRICAN", "WEST AFRICAN"},
            {"WS", "SAMOAN"},
            {"WSM", "SAMOAN"},
            {"XOF", "WEST AFRICAN"},
            {"YE", "YEMENI"},
            {"YEM", "YEMENI"},
            {"YEMEN", "YEMENI"},
            {"YEMENI", "YEMENI"},
            {"ZA", "SOUTH AFRICAN"},
            {"ZAF", "SOUTH AFRICAN"},
            {"ZAMBIA", "ZAMBIAN"},
            {"ZAMBIAN", "ZAMBIAN"},
            {"ZIMBABWE", "ZIMBABWEAN"},
            {"ZIMBABWEAN", "ZIMBABWEAN"},
            {"ZM", "ZAMBIAN"},
            {"ZMB", "ZAMBIAN"},
            {"ZW", "ZIMBABWEAN"},
            {"ZWE", "ZIMBABWEAN"},
        };

        // Check if normalised input resolves in the dictionary
        if (nationalityCodes.ContainsKey(normalisedInput))
        {
            cellData.Data = nationalityCodes[normalisedInput];
            return cellData;
        }
        else
        {
            // Pass through as-is — unknown codes are preserved, not wiped
            cellData.Data = data.Trim();
            return cellData;
        }
    }
    public CellDataAndStatus DateOfBirth(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Errors = new List<string>() { " DATEOFBIRTH: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }
    //TODO :Company names are not allowed
    public CellDataAndStatus Title(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.NewFilterNameByTitle(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    //TODO :Company names are not allowed
    public CellDataAndStatus Surname(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;//stringHelper.NewCleanTextNames(data);
        return cellData;
    }

    //TODO :Company names are not allowed
    public CellDataAndStatus FirstName(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus MiddleNames(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; //stringHelper.NewCleanTextNames(data);
        return cellData;
    }
    public CellDataAndStatus PreviousNames(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; //stringHelper.NewCleanTextNames(data);
        return cellData;
    }
    public CellDataAndStatus Alias(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; //stringHelper.NewCleanTextNames(data);
        return cellData;
    }
    public CellDataAndStatus ProofOfAddType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ProofOfAddNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus CurResAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    //===================================================END++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public CellDataAndStatus DateMovedCurrRes(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus PrevResAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus OwnerOrTenant(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().Replace(" ", "").ToUpper();
        data = data.ToUpper();
        if (data == "0")
        {
            cellData.Data = "O";
            return cellData;
        }
        if (data == "O" || data == "OWNER" || data == "0")
        {
            cellData.Data = "O";
            return cellData;
        }
        if (data == "T" || data == "TENANT")
        {
            cellData.Data = "T";
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostalAddPostCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus EmailAddress(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        data = Regex.Replace(data, @"^https?://", "", RegexOptions.IgnoreCase);
        // Remove trailing slashes or backslashes
        data = data.TrimEnd('/', '\\');
        bool isemailValid = Regex.IsMatch(data, emailPattern);
        if (!string.IsNullOrWhiteSpace(data) && isemailValid)
        {
            data = data.ToLower();
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus MobileTel1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus MobileTel2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NumOfDependants(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        if (int.TryParse(data, out int num) && num > 0)
        {
            data = int.Parse(data).ToString();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    //TODO? default
    public CellDataAndStatus EmpType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }

    //EE
    public CellDataAndStatus EmpPayrollNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Paypoint(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //EE
    public CellDataAndStatus EmpName(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.Replace(",", " ").Replace("~", " ").Replace("`", " ").Replace(";", " ").Replace("!", " ").Replace("@", " ").Replace("#", " ").Replace(":", " ")
            .Replace("$", " ").Replace("%", " ").Replace("^", "").Replace("&", " & ").Replace("*", " ").Replace("_", " ").Replace("+", " ").Replace(".", " ");
        return cellData;
    }
    public CellDataAndStatus EmpAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty; ;
        return cellData;
    }
    public CellDataAndStatus DateOfEmp(string data, string cellHeader)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus Occupation(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.ValidateOccupation(data);
        data = stringHelper.ReplaceAlphanumericWithSpace(data);
        if (data.Length > 1)
        {
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
        }
        return cellData;
    }
    public CellDataAndStatus IncomeCurrency(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Income(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus JointOrSoleAcc(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NoParticipantsInAcc(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldCustomerID(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldAccountNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    public CellDataAndStatus OldSRN(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldBranchCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    //Default: 122

    //TODO? default or empty string
    public CellDataAndStatus CreditFacilityType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        // Only these 8 codes have a fixed CreditFacilityType. Every other
        // code/value, and blanks, default to "P" -- final for individual
        // files (unlike business, individual files have no bank/non-bank
        // resolution step downstream).
        if (data == "102" || data == "AUTOLOAN" || data == "A") { data = "A"; }
        else if (data == "103" || data == "BANKGUARANTEE" || data == "Q") { data = "Q"; }
        else if (data == "106" || data == "CREDITCARD" || data == "C") { data = "C"; }
        else if (data == "109" || data == "HOUSINGLOAN" || data == "H") { data = "H"; }
        else if (data == "111" || data == "LETTEROFCREDIT" || data == "Y") { data = "Y"; }
        else if (data == "118" || data == "MORTGAGE" || data == "H") { data = "H"; }
        else if (data == "121" || data == "OVERDRAFT" || data == "V") { data = "V"; }
        else if (data == "128" || data == "STUDENTLOAN" || data == "STUDENT" || data == "T") { data = "T"; }
        else
        {
            data = "P";
        }
        cellData.Data = data;
        return cellData;
    }
    // private string RemoveSpaces(string input) => string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
    //TODO? default
    public CellDataAndStatus PurposeOfFacility(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if (data == "A" || data == "CRISISLOAN" || data == "CRISIS" || data == "CRISIS") { data = "A"; }
        else if (data == "B" || data == "HOME" || data == "HOMELOANS") { data = "B"; }
        else if (data == "S" || data == "STUDY" || data == "STUDYLOAN") { data = "S"; }
        else if (data == "C" || data == "OTHERASSETACQUISITIONFINANCING") { data = "C"; }
        else if (data == "D" || data == "PROJECT" || data == "PROJECTFINANCE") { data = "D"; }
        else if (data == "E" || data == "CAPITAL" || data == "CAPITALFINANCE") { data = "E"; }
        else if (data == "F" || data == "MACHINERY" || data == "EQUIPMENT" || data == "EQUIPMENTANDMACHINERYFINANCE") { data = "F"; }
        else if (data == "G" || data == "WORKING" || data == "CAPITAL" || data == "WORKINGCAPITALFINANCE") { data = "G"; }
        else if (data == "H" || data == "SUBSCRIPTIONFINANCE" || data == "SUBSCRIPTION") { data = "H"; }
        else if (data == "P" || data == "PERSONAL" || data == "PERSONALFINANCE") { data = "P"; }
        else if (data == "J" || data == "FINANCEFORTRADING" || data == "FINANCEINSECURITIES" || data == "FORTRADING" || data == "INSECURITIES" ||
                data == "FINANCEFORTRADINGINSECURITIES") { data = "J"; }
        else if (data == "K" || data == "CONSOLIDATION" || data == "CONSOLIDATIONLOAN") { data = "K"; }
        else if (data == "L" || data == "OTHER") { data = "L"; }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus FacilityTerm(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        if (string.IsNullOrWhiteSpace(data))
            return new CellDataAndStatus(string.Empty); // Return empty if input is null or whitespace

        // Normalize input: trim and convert to lowercase for easier matching
        string normalizedData = data.Trim().ToLower();

        // Match patterns like "10 months", "10 mths", "10 m", "2 years", "5 days", etc.
        var match = Regex.Match(normalizedData, @"(\d+)\s*(days?|d|weeks?|w|months?|m|years?|y)", RegexOptions.IgnoreCase);

        if (match.Success)
        {
            // Extract the number and the unit
            double value = double.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value;

            // Determine the conversion factor based on the unit
            if (unit.StartsWith("d")) // Days to months — round up so e.g. 28 days = 1 month, not 0
            {
                cellData.Data = ((int)Math.Ceiling(value / 30.44)).ToString();
            }
            else if (unit.StartsWith("w")) // Weeks to months — round up so partial months count as 1
            {
                cellData.Data = ((int)Math.Ceiling((value * 7) / 30.44)).ToString();
            }
            else if (unit.StartsWith("m")) // Already in months
            {
                cellData.Data = ((int)value).ToString();
            }
            else if (unit.StartsWith("y")) // Years to months
            {
                cellData.Data = ((int)(value * 12)).ToString();
            }
            else
            {
                cellData.Data = ((int)value).ToString(); // Default to treating as months
            }

            return cellData;
        }

        // If input doesn't match expected pattern, attempt to extract any number
        var numberOnlyMatch = Regex.Match(normalizedData, @"\d+");
        if (numberOnlyMatch.Success)
        {
            double number = double.Parse(numberOnlyMatch.Value);
            cellData.Data = ((int)number).ToString(); // Treat as months by default
            return cellData;
        }

        return new CellDataAndStatus(string.Empty); // Return empty if no valid numeric data found
    }
    public CellDataAndStatus DefPaymentStartDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus AmountCurrency(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper();
        if (data == "GH" || data == "GHC" || data == "CEDIS" || data == "GHA" || data == "GHS")
        {
            data = "GHS";
        }//GHS, USD, EUR, GBP
        else if (data == "USD" || data == "EUR" || data == "GBP")
        {
            data = data.ToUpper();
        }
        else
        {
            data = string.Empty;
            cellData.Passed = false;
        }
        cellData.Data = data;
        return cellData;
    }

    //(FacilityAmount-DisbursementAmt)
    public CellDataAndStatus FacilityAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; ;
        return cellData;
    }

    public CellDataAndStatus DisbursementDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Errors = new List<string>() { " DISBURSEMENTDATE: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }

    public CellDataAndStatus DisbursementAmt(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    public CellDataAndStatus MaturityDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Errors = new List<string>() { " MATURITYDATE: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }

    public CellDataAndStatus SchdInstalAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "").Replace(",","");


        data = stringHelper.NormalizeDecimalOrComma(data);
        cellData = stringHelper.CleanToTwoDecimalPlaces(data);

        var F_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;
        var D_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;

        return cellData;
    }

    //Negative remove
    public CellDataAndStatus RepaymentFreq(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper().Replace(" ","").Trim();

        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty; // Return an empty string for null or empty input
        }

        switch (data)
        {
            case "10":
            case "WEEKLY":
            case "01":
                data = "01";
                break;

      
            case "11":
            case "BI-MONTHLY":
            case "BIMONTHLY":
            case "02":
                data = "02";
                break;

            case "12":
            case "MONTHLY":
            case "03":
                data = "03";
                break;

            case "13":
            case "QUARTERLY":
            case "04":
                data = "04";
                break;

            case "14":
            case "TRIANNUALLY":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "15":
            case "SEMIANNUALLY":
                data = "05";
                break;

            case "16":
            case "ANNUAL":
                data = "06";
                break;

            case "17":
            case "VARIABLE":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "18":
            case "BULLET":
                data = "07";
                break;

            case "DEMAND":
            case "19":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "20":
            case "UNSPECIFIED":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "21":
            case "BALLOON":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            default:
                data = string.Empty;
                break;
        }
        cellData.Data = data;
        return cellData;
    }

    public CellDataAndStatus LastPaymentAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "").Replace(",", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        cellData = stringHelper.CleanToTwoDecimalPlaces(data);

        var F_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;
        var D_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;

        return cellData;
    }

    public CellDataAndStatus LastPaymentDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }

    public CellDataAndStatus NextPaymentDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }

    public CellDataAndStatus CurBal(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    public CellDataAndStatus CurBalIndicator(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = "D";
        cellData.Data = data;
        return cellData;
    }

    public CellDataAndStatus AssetClassification(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Trim().Replace(" ", "").Replace("–", "-");
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();
        if (string.IsNullOrWhiteSpace(data))
        {
            data = string.Empty;
        }

        // Normalize input to lowercase string for easy matching
        string value = data.ToString().Trim().ToLower();

        // Try to parse an integer if entered
        if (int.TryParse(value, out int days))
        {
            if (days >= 1 && days <= 30) data = "A";
            if (days >= 31 && days <= 90) data = "B";
            if (days >= 91 && days <= 180) data = "C";
            if (days >= 181 && days <= 360) data = "D";
            if (days > 360) data = "E";
        }

        // Handle various string inputs (full and short forms)
        if (value.Contains("current") || value.Contains("a")) data = "A";
        else if (value.Contains("olem") || value.Contains("b")) data = "B";
        else if (value.Contains("sub") || value.Contains("c")) data = "C";
        else if (value.Contains("doubt") || value.Contains("d")) data = "D";
        else if (value.Contains("loss") || value.Contains("over 360") || value.Contains("e")) data = "E";

        // Handle common variations
        else if (value.Contains("1-30") || value.Contains("1to30")) data = "A";
        else if (value.Contains("31-90") || value.Contains("31to90")) data = "B";
        else if (value.Contains("91-180") || value.Contains("91to180")) data = "C";
        else if (value.Contains("181-360") || value.Contains("181to360")) data = "D";
        else
        {
            data = string.Empty; // Return empty if no match
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus AmountInArrears(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ArrearsStartDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NDIA(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.RoundUpNumberOfDaysInArr(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PaymentHistoryProfile(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Trim().Replace(" ", "");
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();

        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }

        // Normalize to lowercase for string matching
        string value = data.Trim().ToLower();

        // ── Valid final codes (0–5) passed directly by subscriber — preserve as-is ──
        // Must be checked BEFORE the day-range integer parse to avoid misreading "1"–"5"
        // as day counts (1 day, 2 days, etc.) and converting them incorrectly.
        var validFinalCodes = new HashSet<string> { "0", "1", "2", "3", "4", "5" };
        if (validFinalCodes.Contains(value))
        {
            cellData.Data = value;
            return cellData;
        }

        // ── Day-count integers: convert to standard code ──────────────────────────
        // Subscribers sometimes send the number of days in arrears rather than the code.
        if (int.TryParse(value, out int days))
        {
            if (days >= 1 && days <= 30)        cellData.Data = "0";
            else if (days >= 31 && days <= 60)  cellData.Data = "1";
            else if (days >= 61 && days <= 90)  cellData.Data = "2";
            else if (days >= 91 && days <= 120)  cellData.Data = "3";
            else if (days >= 121 && days <= 180) cellData.Data = "4";
            else if (days > 180)                 cellData.Data = "5";
            else                                 cellData.Data = string.Empty;
            return cellData; // early return — integer input fully handled
        }

        // ── Descriptive string inputs ─────────────────────────────────────────────
        if      (value.Contains("current") || value.Contains("1-30")    || value.Contains("1to30"))    cellData.Data = "0";
        else if (value.Contains("31-60")   || value.Contains("31to60"))                                cellData.Data = "1";
        else if (value.Contains("61-90")   || value.Contains("61to90"))                                cellData.Data = "2";
        else if (value.Contains("91-120")  || value.Contains("91to120"))                               cellData.Data = "3";
        else if (value.Contains("121-180") || value.Contains("121to180"))                              cellData.Data = "4";
        else if (value.Contains("181")     || value.Contains("181to360") || value.Contains("over 180")) cellData.Data = "5";
        else
        {
            // Input does not match any known pattern — preserve original rather than silently wiping
            cellData.Data = data.Trim();
        }

        return cellData;





    }
    public CellDataAndStatus AmtOverdue1to30days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue31to60days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue61to90days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue91to120days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue121to150days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }

    public CellDataAndStatus AmtOverdue151to180days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue181orMore(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus LegalFlag(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();
        if (data == "101" || data == "YES")
        {
            cellData.Data = "101";
            return cellData;
        }
        if (data == "102" || data == "NO")
        {
            cellData.Data = "102";
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    //string statustype, string currentBalance, string amountOverdue, string writtenOffAmount, string accountInArrears, string nDIA
    public CellDataAndStatus FacilityStatusCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus FacilityStatusDate(string data, string _filename)
    {
        if (!string.IsNullOrWhiteSpace(data))
        {

        }
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        var file_date_LastDate = stringHelper.GetFacilityDateFromFileName(_filename).LastDate;
        var facility_date = stringHelper.CheckDate(data);
        var file_date = stringHelper.CheckDate(file_date_LastDate);
        if ((facility_date.IsValidFormat && file_date.IsValidFormat) && facility_date.TheDate > file_date.TheDate)
        {
            cellData.Data = file_date_LastDate;
            return cellData;
        }
        else if (facility_date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
        //var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        //var date = stringHelper.CheckDate(data);

        //if (date.IsValidFormat)
        //{
        //    cellData.Data = data;
        //    return cellData;
        //}
        //else
        //{

        //    cellData.Data = string.Empty;
        //    return cellData;
        //}
    }
    public CellDataAndStatus ClosedDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }

        // Attempt to normalise common subscriber date formats into yyyyMMdd
        // before passing to CheckDate(), which only accepts yyyyMMdd.
        // Formats handled: yyyy-MM-dd, dd/MM/yyyy, dd-MM-yyyy, MM/dd/yyyy,
        //                  yyyyMMdd (already correct), dd MMM yyyy, etc.
        string normalised = TryNormaliseDateToYyyyMMdd(data.Trim()) ?? data.Trim();

        var date = stringHelper.CheckDate(normalised);
        if (date.IsValidFormat)
        {
            cellData.Data = normalised; // always store in yyyyMMdd
            return cellData;
        }

        // Not a recognisable date — preserve original value rather than silently wiping
        cellData.Data = data.Trim();
        return cellData;
    }

    /// <summary>
    /// Attempts to parse a date string in various common formats and return it
    /// as yyyyMMdd. Returns null if no recognised format matches.
    /// </summary>
    private string? TryNormaliseDateToYyyyMMdd(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        string[] formats = new[]
        {
            "yyyyMMdd",       // already correct — 20240115
            "yyyy-MM-dd",     // ISO 8601   — 2024-01-15
            "yyyy/MM/dd",     // slash ISO  — 2024/01/15
            "dd/MM/yyyy",     // UK style   — 15/01/2024
            "dd-MM-yyyy",     // UK dashes  — 15-01-2024
            "MM/dd/yyyy",     // US style   — 01/15/2024
            "dd MMM yyyy",    // verbose    — 15 Jan 2024
            "dd MMMM yyyy",   // full month — 15 January 2024
            "d/M/yyyy",       // no-pad UK  — 5/1/2024
            "d-M-yyyy",       // no-pad dash
        };

        if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime parsed))
        {
            return parsed.ToString("yyyyMMdd");
        }

        // Fallback: let .NET try any recognised format
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime fallback))
        {
            return fallback.ToString("yyyyMMdd");
        }

        return null;
    }
    public CellDataAndStatus ClosureReason(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if(data == "A" || data == "BYCREDITGRANTORWITHOUTPREJUDICETOTHESUBJECT") { data = "A"; }
        else if(data == "B" || data == "BALANCETRANSFER") { data = "B"; }
        else if(data == "C" || data == "DEATH") { data = "C"; }
        else if(data == "D" || data == "ENDOFCREDITFACILITYTENURE") { data = "D"; }
        else if(data == "E" || data == "MERGEROFCREDITFACILITY") { data = "E"; }
        else if(data == "F" || data == "EARLYSETTLEMENTBYSUBJECT") { data = "F"; }
        else if(data == "G" || data == "BYCOURTORDER") { data = "G"; }
        else if(data == "H" || data == "LOSTCARDS/COMPROMISEDCARDS") { data = "H"; }
        else if(data == "J" || data == "BANKRUPTCY") { data = "J"; }
        else if(data == "K" || data == "RESTRUCTURED/RESCHEDULED") { data = "K"; }

        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }

    //CurrentBalance, AmountOverdue, WrittenOffAmount, AccountInArrears, NDIA"
    public CellDataAndStatus WrittenOffAmt(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ReasonForWrittenOff(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if(data == "A" || data == " PARTSETTLEMENT") { data = "A"; }
        else if(data == "B" || data == "DEATH") { data = "B"; }
        else if(data == "C" || data == "UNABLETOLOCATE") { data = "C"; }
        else if(data == "D" || data == "GOVERNMENTCONCESSION") { data = "D"; }
        else if(data == "E" || data == "BANKRUPTCY") { data = "E"; }
        else if(data == "F" || data == "OTHER") { data = "F"; }



        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus DateRestructured(string data, string f_code)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (f_code == "E" || f_code == "R")
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }

    }
    public CellDataAndStatus ReasonForRestructure(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if(data == "T" || data == "REQUESTFORTOPUPS") { data = "T"; }
        else if(data == "E" || data == "IRREGULARREPAYMENTS") { data = "E"; }
        else if(data == "L" || data == "LOSSOFJOB") { data = "L"; }
        else if(data == "D" || data == "BUSINESSDOWNTURN") { data = "D"; }
        else if(data == "F" || data == "FORCEMAJEURE") { data = "F"; }
        else if(data == "C" || data == "OTHER") { data = "C"; }


        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CreditCollateralInd(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().ToUpper().Replace(" ", "");
        if (data == "Y" || data == "101" || data == "YES")
        {
            data = "101";
            cellData.Data = data;
            return cellData;
        }
        if (data == "N" || data == "102" || data == "NO")
        {
            data = "102";
            cellData.Data = data;
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus SecurityType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper().Trim().Replace(" ", "");

        switch (data)
        {
            case "LAND":
            case "A":
                data = "A";
                break;

            case "SHARES":
            case "B":
                data = "B";
                break;

            case "GOVERNMENTBONDS":
            case "GOVERNMENTSECURITIES":
            case "C":
                data = "C";
                break;

            case "BUILDING":
            case "D":
                data = "D";
                break;

            case "E":
            case "CASH":
            case "FIXEDDEPOSIT":
                data = "E";
                break;

            case "BANKGUARANTEE":
            case "F":
                data = "F";
                break;

            case "SALARYASSIGNMENT":
            case "G":
                data = "G";
                break;

            case "TERMINALBENEFITSASSIGNMENT":
            case "H":
                data = "H";
                break;

            case "BULLIONS":
            case "J":
                data = "J";
                break;

            case "GENERALPLANT&MACHINERY":
            case "K":
                data = "K";
                break;

            case "VEHICLES":
            case "L":
                data = "L";
                break;

            case "CORPORATEGUARANTEE":
            case "M":
                data = "M";
                break;

            case "INDIVIDUALGUARANTEE":
            case "INDIVIDUAL":
            case "N":
                data = "N";
                break;

            case "GOVERNMENTGUARANTEE":
            case "P":
                data = "P";
                break;

            case "OTHERS":
            case "Q":
                data = "Q";
                break;

            default:
                data = string.Empty;
                break;
        }

        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NatureOfCharge(string data)
    {
        data = data.ToUpper();
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (data == "FIXED" || data == "OUTRIGHT" || data == "OUTRIGHTPAYMENT" || data == "A")
        {
            cellData.Data = "FIXED";
            return cellData;
        }
        if (data == "FLOAT" || data == "INSTALMENTS" || data == "B")
        {
            cellData.Data = "FLOAT";
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus SecurityValue(string data) 
    { 
        var cellData = new CellDataAndStatus(data); 
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "");
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; 
        return cellData; 
    }
    public CellDataAndStatus CollRegRefNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus SpecialCommentsCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        HashSet<string> targetNumbers = new HashSet<string> { "101", "102", "103", "104", "105", "106", "107", "108", "109", "110", "111", "112", "113", "114", "115" };

        // Assume 'data' is the variable you want to check
        if (targetNumbers.Contains(data))
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NatureOfGuarantor(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Define the codes and their descriptions in a Dictionary for fast lookups
        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           { "101", "INDIVIDUAL" },
 { "102", "COMMERCIALENTITY"},
 { "103", "NOGUARANTOR" }

        };
        var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in codes)
        {
            descriptions[pair.Value] = pair.Key;
        }
        // Check if the input matches a code or description
        if (codes.TryGetValue(data, out string? description))
        {
            data = data.ToLower();
        }
        else if (descriptions.TryGetValue(data, out string? code))
        {
            data = code;
        }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }

    // data = stringHelper.RemoveSystemErroNames(data);

    //================================================================================================================================================
    public CellDataAndStatus NameOfComGuarantor(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        //data = stringHelper.ConvertBusinessShortFormToLongForm(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus BusRegOfGuarantor(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
            data = string.Empty;

        if (data.Length >= 5 && Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && !Regex.IsMatch(data, @"^[a-zA]+$"))
        {
            data = data.ToUpper();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            data = string.Empty;
            cellData.Data = data;
            return cellData;
        }
       
    }
    public CellDataAndStatus G1Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1Gender(string data) 
    { 
        var cellData = new CellDataAndStatus(data); 
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
        return cellData;
    }
    public CellDataAndStatus G1DOB(string data) 
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G1Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G1WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G1Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
        return cellData;
    }
    public CellDataAndStatus G2DOB(string data) 
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G2Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G2WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G2Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
        return cellData;
    }
    public CellDataAndStatus G3DOB(string data) 
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G3Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G3WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G3Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
        return cellData;
    }
    public CellDataAndStatus G4DOB(string data) 
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G4Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G4WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G4Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
}
