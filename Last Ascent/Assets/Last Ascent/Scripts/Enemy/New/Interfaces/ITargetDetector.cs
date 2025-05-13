public interface ITargetDetector
{
  Player NearestPlayer { get; }
  void TickDetection();
}