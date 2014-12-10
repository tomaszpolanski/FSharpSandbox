let imHungry : Unit = 
    let rec imHungryRec visited = 
        match LuncherConsole.Luncher.GetNotVisitedRestaurants visited with
        | [] -> 
            printfn "Nothing left, you are sooo picky!"
            printf "Try again."
            System.Console.ReadLine() |> ignore
            imHungryRec []
        | x :: xs -> 
            printf "How about %A?" x
            System.Console.ReadLine() |> ignore
            imHungryRec (x :: visited)
    imHungryRec []

[<EntryPoint>]
let main argv = imHungry; 0 // return an integer exit code
