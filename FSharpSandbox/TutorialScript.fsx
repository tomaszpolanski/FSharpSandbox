#load "../packages/FSharp.Charting.0.90.7/FSharp.Charting.fsx"

open FSharp.Charting

#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#load "../Portable/Extensions.fs"

open FSharp.Data

#load "../Portable/Portable.fs"


module ``ListTest`` =
    
    let list = [1..10]

    let oddList = List.filter (fun x -> x % 2 = 1) list

    let addOne x = x + 1

    let twoPlusOne = addOne 2

    let otherTwoPlusOne = 2 |> addOne

    let squareList = list |> List.map (fun x -> x * x)

    let oddSquareList = list |> List.map (fun x -> x * x) 
                             |> List.filter (fun x -> x % 2 = 1)

module ``Lets do some charting`` = 

    let squareList = [-10..10] |> List.map (fun x -> (x, x * x))

    Chart.Line(squareList)

    let numbers = [0..100] |> List.countBy (fun x -> x % 2 = 0)

    Chart.Pie(numbers)

module ``Lets do some useful charting`` = 
    
    let worldData = WorldBankData.GetDataContext()

    Chart.Line(worldData.Countries.Poland.Indicators.``Armed forces personnel, total``)


    let countries = [worldData.Countries.Poland; worldData.Countries.Germany ];

    let armyPersonel = [ for country in countries -> country.Name, country.Indicators.``Armed forces personnel, total``.[2008] ]

    Chart.Doughnut(armyPersonel);

module ``Lets do some advance charting`` =
    
    let worldData = WorldBankData.GetDataContext()

    let countries = 
        [worldData.Countries.Poland; 
         worldData.Countries.Germany;
         worldData.Countries.Finland;
         worldData.Countries.Australia ];

    Chart.Combine([ for country in countries -> Chart.Line(country.Indicators.``Birth rate, crude (per 1,000 people)``, Name=country.Name) ])
     .WithTitle("Birth rate, crude (per 1,000 people)") 
     .WithLegend()

module ``Other providers`` = 

    let freebase =  FreebaseData.GetDataContext();

    (freebase.Transportation.Automotive.``Automobile Companies``.IndividualsAZ.V.``Volkswagen Group``.``Board members`` |> Seq.toList ).[0].Person

    (freebase.Transportation.Automotive.``Automobile Companies``.IndividualsAZ.V.``Volkswagen Group``.Brands |> Seq.toList ).[0].Brand

module ``Comma separated values`` = 

    type data = CsvProvider<"http://ichart.finance.yahoo.com/table.csv?s=NOK">
    let nokia = data.Load("http://ichart.finance.yahoo.com/table.csv?s=NOK")

    nokia.Rows |> Seq.take 1

    let ``M$`` = data.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT")

    let msData = ``M$``.Rows |> Seq.map (fun day -> day.Date, day.Open)
    let nokData = nokia.Rows |> Seq.map (fun day -> day.Date, day.Open)

    let charts = [msData; nokData] |> Seq.map  Chart.FastLine

    charts |> Chart.Combine

module ``Everybody loves recursion`` =
    let rec factorial number = 
       if number = 0 then 1 else factorial (number - 1) * number

    factorial 4

    let rec quickSort list =
        match list with
            | [] -> []
            | first::rest ->
                let smallerElements = 
                    rest |> List.filter (fun item -> item < first)
                         |> quickSort
                let largerElements = 
                    rest |> List.filter (fun item -> item >= first)
                         |> quickSort

                smallerElements @ [first] @ largerElements

    printfn "%A" (quickSort [1; 5; 23; 18; 9; 1; 3])

   
    let fib = Portable.Recursive.Fibonnaci |> Seq.take 50 |> Seq.filter (fun x -> x % 2 = 0) |> Seq.toList
    

module ``Luncher`` = 
    
    let rnd = System.Random()
    let rec getRestaurants (l : string list) =
        seq { 
              if l.Length <> 0 then
                  let number =  l.[rnd.Next(l.Length)]
                  yield  number
                  yield! getRestaurants (l |> List.filter ((<>) number))
            }

    let restuarants = ["Cheap asian";
                       "Belegschaft";
                       "Essian"; 
                       "Karmel"; 
                       "Otito"; 
                       "Corner pizza"; 
                       "Pastadeli"; 
                       "Avan"; 
                       "Soup kultur"; 
                       "Mediterrenian buffe"]; 

    restuarants |> getRestaurants |> Seq.toList


    let rec getNotVisitedRestaruants allRestaurants visited  = 
        match allRestaurants with
        | [] -> []
        | x::xs -> 
            if visited |> List.exists ((=) x)  
            then
                getNotVisitedRestaruants xs visited
            else 
                x :: getNotVisitedRestaruants xs visited

    let visitedRestaurants = [ "Karmel"; 
                               "Otito"; 
                               "Corner pizza"; 
                               "Pastadeli"; ]

    let restarurantsToGoTo = visitedRestaurants |> getNotVisitedRestaruants restuarants  

    /////////
    let randomRestaurants = restuarants |> getRestaurants |> Seq.toList 

    let getNVR l = 
        getNotVisitedRestaruants randomRestaurants l

    visitedRestaurants |> getNVR

    /////////

module ``Luncher on steroids`` =
    type data = CsvProvider<"Restaurants.csv", ";">
    let restaurantData = data.Load("Restaurants.csv")

    restaurantData.Rows |> Seq.sortBy (fun r -> r.Stars)
                        |> Seq.map (fun r -> r.Name, r.Stars)
                        |> Chart.Bar

    /////////
    let cvs = restaurantData.Rows |> Seq.map (fun r -> r.Name) |> Seq.toList |> ``Luncher``.getRestaurants |> Seq.toList 

    ``Luncher``.visitedRestaurants |> ``Luncher``.getNotVisitedRestaruants cvs