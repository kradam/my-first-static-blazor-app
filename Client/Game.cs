using System;
public class Player
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int PairNumber() => (Id % 2 == 1 ? global::PairNumber.First : global::PairNumber.Second);
    public string TdCssClass() => PairNumber() == global::PairNumber.First ? "td-pair-first" : "td-pair-second"; // TODO questionable if it should be here
}
public class PlayerTrick
{
    public int PlayerId { get; set; }
    public int? Plan { get; set; }
    public int? Gain { get; set; }
    public string TdCssClass() => PlayerId % 2 == 1 ? "td-pair-first" : "td-pair-second";
}
public static class PairNumber 
{
    public const int First = 0;
    public const int Second = 1;
}

public class PairScore
{
    public int? score;
    public int? overtrick;
}

public enum GameState { Planning, Scoring, Done}
public class Game
{
    const int TotalTricks = 13;
    public readonly int Number; // { get; set; }
    public int Dealer { get; set; }

    public PlayerTrick[] PlayersTricks { get; set; }
    public PairScore[] PairScores { get; set; }
    public GameState? State { get; set; }
    public Game (int gameNumber, int dealer)
    {
        Number = gameNumber;
        Dealer = dealer;
        State = GameState.Planning;
        PlayersTricks = new PlayerTrick[]
        {
                new PlayerTrick {PlayerId = 1},
                new PlayerTrick {PlayerId = 2},
                new PlayerTrick {PlayerId = 3},
                new PlayerTrick {PlayerId = 4}
         };
        PairScores = new PairScore[]
        {
                new PairScore {score = null, overtrick = null},
                new PairScore {score = null, overtrick = null}
        };
    }
    public bool ToggleStateEnabled(bool penultimateGame) =>
        (State == GameState.Planning && PlayersTricks[0].Plan != null && PlayersTricks[1].Plan != null && PlayersTricks[2].Plan != null && PlayersTricks[3].Plan != null) ||
        (State == GameState.Scoring) ||
        (State == GameState.Done && penultimateGame);
    public void ToggleState()
    {
        switch(State)
        {
            case GameState.Planning:
                State = GameState.Scoring;
                break;
            case GameState.Scoring:
                State = GameState.Planning;
                break;
            case GameState.Done:
                State = GameState.Scoring;
                break;
        }
    }
    public bool PlanAndGainValidated() =>
        (PlayersTricks[0].Plan != null) && (PlayersTricks[1].Plan != null) && (PlayersTricks[2].Plan != null) && (PlayersTricks[3].Plan != null) &&
        (PlayersTricks[0].Gain != null) && (PlayersTricks[1].Gain != null) && (PlayersTricks[2].Gain != null) && (PlayersTricks[3].Gain != null) &&
        (SumGain() == TotalTricks);
    public bool GainIncorrect() => (PlayersTricks[0].Gain != null) && (PlayersTricks[1].Gain != null) && (PlayersTricks[2].Gain != null) && (PlayersTricks[3].Gain != null) && (SumGain() != TotalTricks);
    public int SumPlan() => (PlayersTricks[0].Plan ?? 0) + (PlayersTricks[1].Plan ?? 0) + (PlayersTricks[2].Plan ?? 0) + (PlayersTricks[3].Plan ?? 0);
    public int SumGain() =>   (PlayersTricks[0].Gain ?? 0) + (PlayersTricks[1].Gain ?? 0) + (PlayersTricks[2].Gain ?? 0) + (PlayersTricks[3].Gain ?? 0);
    public int PairPlan(int pair) => (PlayersTricks[pair].Plan ?? 0) + (PlayersTricks[pair + 2].Plan ?? 0);
    public int PairGain(int pair) => (PlayersTricks[pair].Gain ?? 0) + (PlayersTricks[pair + 2].Gain ?? 0);
    public string PairScore(int pair) => PairScores[pair].score != null ? PairScores[pair].score.ToString() : "-";
    public string PairOvertrick(int pair) => PairScores[pair].overtrick != null ? PairScores[pair].overtrick.ToString() : "-";
    public void CalculateScore(Game previousGame = null)
    {
        CalculatePairScore(PairNumber.First, previousGame);
        CalculatePairScore(PairNumber.Second, previousGame);
    }
    public bool CalculatePairScore(int pair, Game previousGame)
    {
        if (previousGame == null)
        {
            PairScores[pair].score = PairScores[pair].overtrick = 0;
        }
        else
        {
            PairScores[pair].score = previousGame.PairScores[pair].score;
            PairScores[pair].overtrick = previousGame.PairScores[pair].overtrick;
        }

        int player1Idx = pair, player2Idx = pair + 2;
        if (PlayersTricks[player1Idx].Plan == null || PlayersTricks[player1Idx].Gain == null ||
            PlayersTricks[player2Idx].Plan == null || PlayersTricks[player2Idx].Gain == null)
        {
            PairScores[pair].score = PairScores[pair].overtrick = null;
            return false;
        }

        foreach (int playerIdx in new int[] { player1Idx, player2Idx })
            if (PlayersTricks[playerIdx].Plan == 0)
            {
                PairScores[pair].score += (PlayersTricks[playerIdx].Gain == 0) ? +100 : -100;
                //Console.WriteLine(playerIdx);
            }
        int pairPlan = (int)PlayersTricks[player1Idx].Plan + (PlayersTricks[player2Idx].Plan ?? 0);
        int pairGain = (PlayersTricks[player1Idx].Gain ?? 0) + (PlayersTricks[player2Idx].Gain ?? 0);
        if (pairGain >= pairPlan)
        {
            PairScores[pair].score += pairPlan * 10;
            PairScores[pair].overtrick += pairGain - pairPlan;
            if (PairScores[pair].overtrick > 9)
            {
                PairScores[pair].score -= 100;
                PairScores[pair].overtrick -= 10;
            }
        }
        else
        {
            PairScores[pair].score -= pairPlan * 10;
        }
        return true;
    }
   
    
}
