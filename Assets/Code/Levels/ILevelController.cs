using Code.VisionCone.Factory;

namespace Code.Levels
{
    public interface ILevelController
    {
        void SetLevelPathMover(LevelPathMover levelPathMover);
        void PlayLevel(VisionType visionType, float timeLevel);
        void Dispose();
    }
}