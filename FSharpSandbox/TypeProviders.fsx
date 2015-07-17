#r """..\packages\FSharp.Data.2.2.3\lib\portable-net40+sl5+wp8+win8\FSharp.Data.dll"""

open FSharp.Data

module ``Json provider`` =

    type json = JsonProvider<"http://api.openweathermap.org/data/2.5/weather?q=warsaw&units=metric">

    let jsonValues = json.GetSample()

    jsonValues.Main.TempMax

module ``World Bank`` =

    let data = WorldBankData.GetDataContext()

    data.Countries.Germany.Indicators.``Adjusted savings: mineral depletion (% of GNI)``.[2013]



