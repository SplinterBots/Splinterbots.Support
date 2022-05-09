﻿namespace Functional.SplinterBots.BattleAPI

open Functional.SplinterBots.API
  
type Team (summoner: Cards.Card, team: Cards.Card seq) =
    let secret = API.generateRandomString 10
    let teamHash =
        let monstersString = 
            team
            |> Seq.map (fun monster -> monster.card_long_id)
            |> String.concat ","
        sprintf "%s,%s,%s"
            summoner.card_long_id
            monstersString
            secret
        |> generateMD5Hash

    member this.Summoner 
        with get () = summoner
    member this.Team 
        with get () = team
    member this.Secret 
        with get () = secret
    member this.TeamHash
        with get () = teamHash
