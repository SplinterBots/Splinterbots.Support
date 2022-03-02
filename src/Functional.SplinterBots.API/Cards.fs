﻿namespace Functional.SplinterBots.API

module Cards =
    open System
    open API


    type Card =
        {
           card_detail_id: int
           gold: bool
           edition: int
           xp: int
        }

    type PlayerCardCollection = 
        {
            player: string
            cards: Card array
        }

    let getPlayerCollection playerName =
        async {
            let uri = getCardsApiUri $"collection/{playerName}"
            let! cards = executeApiCall<PlayerCardCollection> uri

            return cards.cards
        }

    let getCardList () =
        async {
            let uri = getCardsApiUri "get_details"
            let! cards = executeApiCall<PlayerCardCollection> uri

            return cards.cards
        }
        
    let sentCards cardIds playerName activeKey =
        let transactionPayload  =
            sprintf "{\"to\":\"%s\",\"cards\":[%s],\"app\":\"%s\",\"n\":\"%s\"}"
                playerName
                cardIds
        let operations = API.createCustomJsonActiveKey playerName "sm_gift_cards" transactionPayload
        let txid = API.hive.broadcast_transaction([| operations |] , [| activeKey |])
        API.waitForTransaction playerName txid
