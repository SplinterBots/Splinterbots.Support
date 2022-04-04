﻿namespace Functional.SplinterBots.API

module Cards =
    open System
    open API


    type Card =
        {
           card_detail_id: int
           gold: bool
           xp: int
           player: string
           uid: string
        }

    type PlayerCardCollection = 
        {
            player: string
            cards: Card array
        }

    let getPlayerCollection playerName =
        async {
            let uri = getCardsUri $"collection/{playerName}"
            let! cards = executeApiCall<PlayerCardCollection> uri

            return cards.cards
        }

    let filterCardsByOwningPlayer playerName (cards: Card seq) = 
        cards
        |> Seq.filter (fun card -> card.player = playerName)
        
    let sentCards cardIds destinationPlayerName playerName activeKey =
        let transactionPayload  =
            sprintf "{\"to\":\"%s\",\"cards\":[%s],\"app\":\"%s\",\"n\":\"%s\"}"
                destinationPlayerName
                cardIds
        let operations = API.createCustomJsonActiveKey playerName "sm_gift_cards" transactionPayload
        let txid = API.hive.broadcast_transaction([| operations |] , [| activeKey |])
        API.waitForTransaction playerName txid

    type CardType =
        | Monster
        | Splinter

    type CardColour =
        | All
        | Red
        | Blue
        | Green
        | White
        | Black
        | Gold
        | Gray

    [<Flags>]
    type CardRarity  =
        | Common = 1
        | Rare = 2
        | Epic = 3
        | Legendary = 4

    type CardListItem =
        {
            id: int
            name: string
            ``type``: CardType
            color: CardColour
            rarity: CardRarity
            is_starter: bool
        }

    let cardsList = 
        let cardsUri = $"{api2Uri}/cards/get_details"
        let rawCardsData = executeApiCall<CardListItem seq> cardsUri |> Async.RunSynchronously
        rawCardsData

    let getStarterCards () =
          cardsList 
          |> Seq.filter (fun card -> card.is_starter)

