var system = "the fast brown fox jumped over the lazy dog";
var reference = "the quick brown animal jumped over the lazy dog";

var scores = ROUGE.Rouge.GetScores(system, reference, false);

foreach (var score in scores)
    Console.WriteLine($"{score.Key}: " + System.Text.Json.JsonSerializer.Serialize(score.Value));
