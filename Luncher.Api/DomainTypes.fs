namespace Luncher.Api

type RestaurantType = {Name:string}

module Restaurant = 

    let create (restaurantName : string) = {Name = restaurantName.Trim()}
    let Empty = {Name = ""}
    let IsEmpty {Name = name} = name = ""

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