using Application.Interface.Screen;
using Application.Interface.Upgrade;
using Application.Model.Upgrade.Definition.Base;
using Application.Model.Upgrade.Definition.Health;
using Application.Model.Upgrade.Definition.Score;
using Application.Screen;
using Application.Service.Upgrade;
using FlappyIncremental.Dto;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

#region Dependency Injection

#region Services

services.AddSingleton<IUpgradeService, UpgradeService>();

#endregion

#region Screens

services.AddTransient<IScreen, PlayScreen>();
services.AddTransient<IScreen, MenuScreen>();
services.AddTransient<IScreen, UpgradeScreen>();

services.AddTransient<PlayScreen>();
services.AddTransient<MenuScreen>();
services.AddTransient<UpgradeScreen>();

#endregion

#region Upgrades

services.AddTransient<BaseUpgradeModel, ScoreUpgradeModel>();
services.AddTransient<BaseUpgradeModel, HealthUpgradeModel>();

#endregion

#endregion

var provider = services.BuildServiceProvider();

GlobalVariables.ServiceProvider = provider;

GlobalVariables.Game = new FlappyIncremental.Flappy();
GlobalVariables.Game.Run();
