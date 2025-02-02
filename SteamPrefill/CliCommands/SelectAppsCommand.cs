﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using JetBrains.Annotations;
using Spectre.Console;
using SteamPrefill.Models;
using SteamPrefill.Utils;

// ReSharper disable MemberCanBePrivate.Global - Properties used as parameters can't be private with CliFx, otherwise they won't work.
namespace SteamPrefill.CliCommands
{
    [UsedImplicitly]
    [Command("select-apps", Description = "Displays an interactive list of all owned apps.  " +
                                          "As many apps as desired can be selected, which will then be used by the 'prefill' command")]
    public class SelectAppsCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            var ansiConsole = console.CreateAnsiConsole();
            using var steamManager = new SteamManager(ansiConsole, new DownloadArguments());
            try
            {
                
                steamManager.Initialize();
                await steamManager.SelectAppsAsync();

                var runPrefill = ansiConsole.Prompt(new SelectionPrompt<bool>()
                                    .Title(SpectreColors.LightYellow("Run prefill now?"))
                                    .AddChoices(true, false)
                                    .UseConverter(e => e == false ? "No" : "Yes"));
                if (runPrefill)
                {
                    await steamManager.DownloadMultipleAppsAsync(false, new List<uint>());
                }
            }
            catch (Exception e)
            {
                ansiConsole.WriteException(e, ExceptionFormats.ShortenPaths);
            }
            finally
            {
                steamManager.Shutdown();
            }
        }
    }
}
