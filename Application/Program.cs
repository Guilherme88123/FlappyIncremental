using Application.Interface.Menu;
using Application.Service.Menu;
using Microsoft.Extensions.DependencyInjection;
using FlappyIncremental.Dto;
using Application.Interface.Screen;
using Application.Screen;

var services = new ServiceCollection();

services.AddSingleton<IMenuService, MenuService>();

#region Screens

services.AddTransient<IScreen, PlayScreen>();
services.AddTransient<IScreen, MenuScreen>();

#endregion

var provider = services.BuildServiceProvider();

GlobalVariables.ServiceProvider = provider;

GlobalVariables.Game = new FlappyIncremental.Flappy();
GlobalVariables.Game.Run();
