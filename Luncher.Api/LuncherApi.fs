namespace Luncher.Api


open  System
type LuncherApi() =
   static let notVisited (restaurants) (visited : seq<RestaurantType>) = 
        Luncher.Api.Restaurant.distinct (Seq.toList restaurants) visited
   static member GetRestaurants(restaurantString : String) = 
                            restaurantString 
                            |> FileSystem.parseLine
                            |> Seq.map Luncher.Api.Restaurant.create
                            |> Luncher.Api.Restaurant.randomize



   static member  ImHungry (all : seq<RestaurantType>) : seq<RestaurantType> = 
            let rec imHungryRec (visited : RestaurantType list) = 
                seq {
                    match notVisited all visited with
                    | [] -> 
                        //printfn "Nothing left, you are sooo picky!"
                        //printf "Try again."
                        //System.Console.ReadLine() |> ignore
                        yield {name=""}
                        yield! imHungryRec []
                    | x :: xs -> 
                        //printf "How about %A?" x
                        //System.Console.ReadLine() |> ignore
                        yield x
                        yield! imHungryRec (x :: visited)
                }
            imHungryRec []
