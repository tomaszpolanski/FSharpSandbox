module LuncherConsole.Luncher

module private PrivateLuncher = 
    open FSharp.Data
    
    let rnd = System.Random()
    
    let rec randomizeRestaurants (l : string list) = 
        seq { 
            if l.Length <> 0 then 
                let number = l.[rnd.Next(l.Length)]
                yield number
                yield! randomizeRestaurants (l |> List.filter ((<>) number))
        }
    
    let rec getNotVisitedRestaruants allRestaurants visited = 
        match allRestaurants with
        | [] -> []
        | x :: xs -> 
            if visited |> List.exists ((=) x) then getNotVisitedRestaruants xs visited
            else x :: getNotVisitedRestaruants xs visited
    
    type data = CsvProvider< "Restaurants.csv", ";" >
    
    let restaurantData = data.Load("Restaurants.csv")
    
    let Restaurants() : string list = 
        restaurantData.Rows
        |> Seq.map (fun r -> r.Name)
        |> Seq.toList
        |> randomizeRestaurants
        |> Seq.toList

let GetNotVisitedRestaurants visited = PrivateLuncher.getNotVisitedRestaruants (PrivateLuncher.Restaurants()) visited
