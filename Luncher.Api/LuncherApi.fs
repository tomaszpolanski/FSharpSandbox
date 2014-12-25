namespace Luncher.Api

module Test =
    open  System
    type LuncherApi() =
        let notVisited (restaurants) (visited : seq<RestaurantType>) = 
            Luncher.Api.Restaurant.distinct (Seq.toList restaurants) visited
        member this.GetRestaurants(restaurantString : String) = 
                                restaurantString 
                                |> FileSystem.parseLine
                                |> Seq.map Luncher.Api.Restaurant.create
                                |> Luncher.Api.Restaurant.randomize



        member this.ImHungry (all : seq<RestaurantType>) : seq<RestaurantType> = 
                let rec imHungryRec (all : seq<RestaurantType>) (visited : RestaurantType list) = 
                    seq {
                        match notVisited all visited with
                        | [] -> 
                            //printfn "Nothing left, you are sooo picky!"
                            //printf "Try again."
                            //System.Console.ReadLine() |> ignore
                            yield {name=""}
                            yield! imHungryRec all []
                        | x :: xs -> 
                            //printf "How about %A?" x
                            //System.Console.ReadLine() |> ignore
                            yield x
                            yield! imHungryRec all (x :: visited)
                    }
                imHungryRec all []
