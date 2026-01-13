using Application.Interface.Menu;
using Application.Service.Menu;
using Microsoft.Extensions.DependencyInjection;
using FlappyIncremental.Dto;

var services = new ServiceCollection();

services.AddSingleton<IMenuService, MenuService>();

var provider = services.BuildServiceProvider();

GlobalVariables.ServiceProvider = provider;

GlobalVariables.Game = new FlappyIncremental.Flappy();
GlobalVariables.Game.Run();
