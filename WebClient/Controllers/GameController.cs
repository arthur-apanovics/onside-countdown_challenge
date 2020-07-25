﻿using System;
using GameCore;
using GameCore.Enums;
using GameCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebClient.Controllers
{
    public class GameController : Controller
    {
        /// <summary>
        /// Game logic and state.
        /// This implementation supports only a single player, additional players can be supported
        /// by storing state in memory/database and identifying players with a token.
        /// </summary>
        private static readonly Countdown GameCore = new Countdown();

        public GameController()
        {
        }

        [HttpGet]
        public IActionResult GetState()
        {
            return Ok(GameCore.State);
        }

        /// <summary>
        /// Fetches a letter from the game "engine"
        /// </summary>
        /// <param name="param">Defaults to 'Vowel'</param>
        /// <returns>200 with letter of requested type or 409 with exception message</returns>
        [HttpGet]
        public IActionResult GetLetter(LetterType param)
        {
            if (GameCore.State.IsTotalLetterLimitReached())
            {
                return Conflict(
                    new Exception("Letter limit reached"));
            }

            if (GameCore.State.IsLetterLimitReached(param))
            {
                return Conflict(
                    new Exception($"All {param.ToString().ToLower()}s used"));
            }

            return Ok(GameCore.GetLetterAndUpdateStateOrNull(param));
        }

        [HttpPost]
        public IActionResult SubmitGuessAndGetResult(string guess)
        {
            if (GameCore.State.IsRoundLimitReached)
            {
                return Conflict(
                    new Exception("Round limit reached"));
            } 

            return Ok(
                new
                {
                    LongestWord = GameCore.GetLongestWordOrNull(),
                    GuessValid = GameCore.IsValidGuessAndUpdateScore(guess.ToLower()),
                    State = GameCore.StartNewRoundAndGetStateOrNull()
                });
        }

        [HttpPost]
        public IActionResult ResetGame()
        {
            return Ok(GameCore.ResetGame());
        }
    }
}