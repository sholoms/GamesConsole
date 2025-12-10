using Autofac;
using Games.controllers;
using Games.Interfaces;
using Games.Minesweeper;
using Games.Minesweeper.Components;
using Games.Wordle;
using Games.Wordle.Services;

namespace Games;

public class Program
{
    private static Autofac.IContainer Container { get; set; }
    public static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<AppController>().As<IProgramController>();
        
        builder.RegisterType<FileService>().As<IFileService>();
        
        builder.RegisterType<WordClient>().As<IWordClient>();
        
        
        builder.RegisterType<MinesweeperGame>().As<IGame>().AsSelf();
        builder.RegisterType<WordleGame>().As<IGame>().AsSelf();

        builder.Register(ctx => new MinesweeperGame());
        builder.Register(ctx => new WordleGame(ctx.Resolve<IWordClient>()));
        
        Container = builder.Build();
        
        Task t = MainAsync(args);
        t.Wait();
    }
    
    static async Task MainAsync(string[] args)
    {
        await RunApplication(args);
    }

    private static async Task RunApplication(string[] args)
    { 
        await using var scope = Container.BeginLifetimeScope();
        var controller = scope.Resolve<IProgramController>();
        await controller.Run();
    }
}