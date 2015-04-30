#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#load "../packages/FSharp.Charting.0.90.7/FSharp.Charting.fsx"

open FSharp.Charting
open FSharp.Data

[ -10..10 ]
|> List.map (fun x -> x, x * x)
|> Chart.Line

let worldData = WorldBankData.GetDataContext()

[ for ind in worldData.Countries.Germany.Indicators.``Access to electricity (% of population)`` do
      yield match ind with
            | year, per -> sprintf "Year: %d, per %f" year per ]
Chart.Bar(worldData.Countries.Germany.Indicators.``Internet users (per 100 people)``)

[for i in 0..5..100 do yield i,i] |> Chart.Bar