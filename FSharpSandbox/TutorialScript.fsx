#load "../packages/FSharp.Charting.0.90.7/FSharp.Charting.fsx"

open FSharp.Charting

#r "../packages/FSharp.Data.2.1.0/lib/net40/FSharp.Data.dll"
#load "../Portable/Extensions.fs"

open FSharp.Data

#load "../Portable/Portable.fs"

#r "../packages/FsCheck.1.0.4/lib/net45/FsCheck.dll"
open FsCheck  

module ``The basics`` = 
    let x = 2
    let y = x + 3
    let addTwo arg = arg + 2
    
    addTwo 3
    
    let add first second = first + second
    
    add 1 9

module Piping = 
    let printHello what = printfn "Hello %s!" what
    
    printHello "world"
    "earth" |> printHello
    
    let printHello2 say what = printfn "%s %s!" say what
    
    printHello2 "Hi" "world"
    "world" |> printHello2 "Yo"

module ListTest = 
    let list = [ 1..10 ]
    let oddList = List.filter (fun x -> x % 2 = 1) list
    let squareList = list |> List.map (fun x -> x * x)
    
    let oddSquareList = 
        list
        |> List.map (fun x -> x * x)
        |> List.filter (fun x -> x % 2 = 1)

module ``Lets do some charting`` = 
    let squareList = [ -10..10 ] |> List.map (fun x -> (x, x * x))
    
    Chart.Line(squareList)
    
    let numbers = [ 0..100 ] |> List.countBy (fun x -> x % 2 = 0)
    
    Chart.Pie(numbers)

module ``Lets do some useful charting`` = 
    let worldData = WorldBankData.GetDataContext()
    
    Chart.Line(worldData.Countries.Poland.Indicators.``Armed forces personnel, total``)
    
    let countries = [ worldData.Countries.Poland; worldData.Countries.Germany ]
    
    let armyPersonel = 
        [ for country in countries -> country.Name, country.Indicators.``Armed forces personnel, total``.[2008] ]
    
    Chart.Doughnut(armyPersonel)

module ``Lets do some advance charting`` = 
    let worldData = WorldBankData.GetDataContext()
    let countries = 
        [ worldData.Countries.Poland; worldData.Countries.Germany; worldData.Countries.Finland; 
          worldData.Countries.Australia ]
    
    Chart.Combine([ for country in countries -> 
                        Chart.Line(country.Indicators.``Birth rate, crude (per 1,000 people)``, Name = country.Name) ])
         .WithTitle("Birth rate, crude (per 1,000 people)").WithLegend()

module ``Other providers`` = 
    let freebase = FreebaseData.GetDataContext()
    
    (freebase.Transportation.Automotive.``Automobile Companies``.IndividualsAZ.V.``Volkswagen Group``.``Board members`` 
     |> Seq.toList).[0].Person
    (freebase.Transportation.Automotive.``Automobile Companies``.IndividualsAZ.V.``Volkswagen Group``.Brands 
     |> Seq.toList).[0].Brand

module ``Comma separated values`` = 
    type data = CsvProvider< "http://ichart.finance.yahoo.com/table.csv?s=NOK" >
    
    let nokia = data.Load("http://ichart.finance.yahoo.com/table.csv?s=NOK")
    
    nokia.Rows |> Seq.take 1
    
    let ``M$`` = data.Load("http://ichart.finance.yahoo.com/table.csv?s=MSFT")
    let msData = ``M$``.Rows |> Seq.map (fun day -> day.Date, day.Open)
    let nokData = nokia.Rows |> Seq.map (fun day -> day.Date, day.Open)
    let charts = [ msData; nokData ] |> Seq.map Chart.FastLine
    
    charts |> Chart.Combine

module ``Everybody loves recursion`` = 
    let rec factorial number = 
        if number = 0 then 1
        else factorial (number - 1) * number
    
    factorial 4
    
    let rec quickSort list = 
        match list with
        | [] -> []
        | first :: rest -> 
            let smallerElements = 
                rest
                |> List.filter (fun item -> item < first)
                |> quickSort
            
            let largerElements = 
                rest
                |> List.filter (fun item -> item >= first)
                |> quickSort
            
            smallerElements @ [ first ] @ largerElements
    
    printfn "%A" (quickSort [ 1; 5; 23; 18; 9; 1; 3 ])
    [ 1; 5; 23; 18; 9; 1; 3 ]
    |> quickSort
    |> printfn "%A"

module Luncher = 
    let rnd = System.Random()
    
    let rec getRestaurants (l : string list) = 
        seq { 
            if l.Length <> 0 then 
                let number = l.[rnd.Next(l.Length)]
                yield number
                yield! getRestaurants (l |> List.filter ((<>) number))
        }
    
    let restuarants = 
        [ "Cheap asian"; "Belegschaft"; "Essian"; "Karmel"; "Otito"; "Corner pizza"; "Pastadeli"; "Avan"; "Soup kultur"; 
          "Mediterrenian buffe" ]
    
    restuarants
    |> getRestaurants
    |> Seq.toList
    
    let rec getNotVisitedRestaruants allRestaurants visited = 
        match allRestaurants with
        | [] -> []
        | x :: xs -> 
            if visited |> List.exists ((=) x) then getNotVisitedRestaruants xs visited
            else x :: getNotVisitedRestaruants xs visited
    
    let visitedRestaurants = [ "Karmel"; "Otito"; "Corner pizza"; "Pastadeli" ]
    let restarurantsToGoTo = visitedRestaurants |> getNotVisitedRestaruants restuarants
    
    /////////
    let randomRestaurants = 
        restuarants
        |> getRestaurants
        |> Seq.toList
    
    let getNVR l = getNotVisitedRestaruants randomRestaurants l
    
    visitedRestaurants |> getNVR

/////////
module ``Luncher on steroids`` = 
    type data = CsvProvider< "Restaurants.csv", ";" >
    
    let restaurantData = data.Load("Restaurants.csv")
    
    restaurantData.Rows
    |> Seq.sortBy (fun r -> r.Stars)
    |> Seq.map (fun r -> r.Name, r.Stars)
    |> Chart.Bar
    
    /////////
    let cvs = 
        restaurantData.Rows
        |> Seq.map (fun r -> r.Name)
        |> Seq.toList
        |> Luncher.getRestaurants
        |> Seq.toList
    
    Luncher.visitedRestaurants |> Luncher.getNotVisitedRestaruants cvs

module ``Property based testing base`` =

    let areEqual = function
        | (x, y) when x = y ->  printfn "Passed"
        | _ -> printfn "Failed"

    let addProper x y = x + y

    let addV0 x y = 571324987

    let addV1 x y = 3 

    let addV2 x y =
        if x=2 && y=2 then 
            4
        else
            3 

    let addV3 x y =
        if x=1 && y=2 then 
            3
        else if x=2 && y=2 then 
            4
        else
            5  

    let addV4 x y = x * y

    let addV5 x y = 0

    let add = addProper


    let ``When I add 1 + 2, I expect 3``()=
        let result = add 1 2 // v0 v1
        areEqual (3, result)

    ``When I add 1 + 2, I expect 3``()


    let ``When I add 2 + 2, I expect 4``()=
        let result = add 2 2 // v1 v2
        areEqual(4, result)

    ``When I add 2 + 2, I expect 4``()


    let ``When I add 3 + 2, I expect 5``()=
        let result = add 3 2 // v2 v3
        areEqual(5, result)

    ``When I add 3 + 2, I expect 5``()
  
  // let's get smarter
    let rand = System.Random()
    let randInt() = rand.Next(10)

    let ``When I add two random numbers, I expect their sum``()=
        let x = randInt()
        let y = randInt()
        let expected = x + y
        let actual = add x y // proper
        areEqual(expected,actual)

    ``When I add two random numbers, I expect their sum``()

    let ``When I add two random numbers (100 times), I expect their sum``()=
        for _ in [1..100] do
            let x = randInt()
            let y = randInt()
            let expected = x + y
            let actual = add x y  // proper
            areEqual(expected,actual)

    ``When I add two random numbers (100 times), I expect their sum``()

    // Property testing

    let ``When I add two numbers, the result should not depend on parameter order`` x y =
            let result1 = add x y // v3 v4
            let result2 = add y x 
            result1 = result2

    Check.Quick ``When I add two numbers, the result should not depend on parameter order``

    let ``Adding 1 twice is the same as adding 2`` x =
        let result1 = add ( add x 1 ) 1 // v4 v5
        let result2 = add x 2  
        result1 = result2

    Check.Quick ``Adding 1 twice is the same as adding 2``

    let ``Adding zero is the same as doing nothing`` x =
        let result1 = add x 0 // v5
        let result2 = x 
        result1 = result2

    Check.Quick ``Adding zero is the same as doing nothing``