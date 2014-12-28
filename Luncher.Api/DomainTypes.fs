namespace Luncher.Api

open System

type RestaurantType = {Name:string}

[<CLIMutable>]
type PickedRestaurantType = {Restaurant: string; Date: DateTime}

module Restaurant = 

    let create (restaurantName : string) = {Name = restaurantName.Trim()}
    let Empty = {Name = ""}
    let IsEmpty restaurant = restaurant = Empty

    let CreatePicked restaurant = {Restaurant=restaurant.Name; Date = DateTime.Now}

    let rec randomize (l : seq<'T>) = 
        let rnd = System.Random()
        let list = l |> Seq.toList
        seq { 
            if list.Length <> 0 then 
                let item = list.[rnd.Next(list.Length)]
                yield item
                yield! randomize (l |> Seq.filter ((<>) item))
        }

    let rec distinct all existing = 
        match all with
        | [] -> []
        | x :: xs -> 
            if existing |> Seq.exists ((=) x) 
                then distinct xs existing
                else x :: distinct xs existing