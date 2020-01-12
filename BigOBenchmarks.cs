using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace TackleBigONetCore
{
  public class BigOBenchmarks
  {
    private const int N = 999;

    private readonly IList<LabRat> labRats;
    private readonly IList<Maze> mazes;
    private readonly IList<Race> races;

    private readonly IDictionary<int, LabRat> labRatLookup;
    private readonly ILookup<int, Race> raceLookup;
    private readonly IDictionary<int, Maze> mazeLookup;

    public BigOBenchmarks()
    {
      labRats = new List<LabRat>(N);
      for (var i = 0; i < N; i++)
      {
        labRats.Add(new LabRat
        {
          TrackingId = i,
          Color = (Color)(i % 3)
        });
      }

      labRatLookup = labRats.ToDictionary(l => l.TrackingId);

      mazes = new List<Maze>(N / 3);
      for (var i = 0; i < N / 3; i++)
      {
        mazes.Add(new Maze
        {
          MazeNumber = i,
          Difficulty = (Difficulty)(i % 3),
          LabRat1 = i,
          LabRat2 = i + N / 3,
          LabRat3 = i + N / 3 * 2
        });
      }

      mazeLookup = mazes.ToDictionary(l => l.MazeNumber);

      races = new List<Race>(N);
      for (var i = 0; i < N; i++)
      {
        races.Add(new Race
        {
          MazeNumber = i % (N / 3),
          Participant = i,
          FinishTime = (FinishTime)(i % 3)
        });
      }

      raceLookup = races.ToLookup(r => r.MazeNumber, r => r);
    }

    // For best results, try running one benchmark at a time.

    [Benchmark]
    public int BigONLabRats()
    {
      var result = labRats.First(d => d.TrackingId == N - 1);

      return (int)result.Color;
    }

    public int BigONSquareMazes()
    {
      var result = 0;

      foreach (var maze in mazes)
      {
        var rat = labRats.First(r => r.TrackingId == N - 1);

        result = maze.LabRat3 == rat.TrackingId ? (int)maze.Difficulty : result;
      }

      return result;
    }

    public int BigONMazes()
    {
      var result = 0;

      foreach (var maze in mazes)
      {
        var rat = labRatLookup[N - 1];

        result = maze.LabRat3 == rat.TrackingId ? (int)maze.Difficulty : result;
      }

      return result;
    }

    public int BigONCubeRaces()
    {
      var result = 0;

      foreach (var maze in mazes)
      {
        foreach (var rat in labRats)
        {
          var race = races.Where(r => r.MazeNumber == N / 3 - 1).ToArray();

          result = maze.MazeNumber == race[2].MazeNumber
            && rat.TrackingId == race[2].Participant
            ? (int)race[2].FinishTime : result;
        }
      }

      return result;
    }

    public int BigONRaces()
    {
      var result = 0;

      foreach (var maze in mazes)
      {
        var race = raceLookup[N / 3 - 1].ToArray();
        var rat = labRatLookup[race[2].Participant];

        result = maze.MazeNumber == race[2].MazeNumber
          && rat.TrackingId == race[2].Participant
          ? (int)race[2].FinishTime : result;
      }

      return result;
    }

    public int BigOOneRaces()
    {
      var race = raceLookup[N / 3 - 1].ToArray();
      var rat = labRatLookup[race[2].Participant];
      var maze = mazeLookup[race[2].MazeNumber];

      var result = maze.MazeNumber == race[2].MazeNumber
        && rat.TrackingId == race[2].Participant
        ? (int)race[2].FinishTime : 0;

      return result;
    }

    private enum Difficulty
    {
      Easy,
      Medium,
      Hard
    }

    private enum Color
    {
      Black,
      White,
      Gray
    }

    private enum FinishTime
    {
      Fast,
      Average,
      Slow
    }

    private class Maze
    {
      public int MazeNumber { get; set; }
      public Difficulty Difficulty { get; set; }
      public int LabRat1 { get; set; }
      public int LabRat2 { get; set; }
      public int LabRat3 { get; set; }
    }

    private class LabRat
    {
      public int TrackingId { get; set; }
      public Color Color { get; set; }
    }

    private class Race
    {
      public int MazeNumber { get; set; }
      public int Participant { get; set; }
      public FinishTime FinishTime { get; set; }
    }
  }
}
