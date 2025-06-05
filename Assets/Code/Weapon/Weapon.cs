using DG.Tweening;
using UnityEngine;

namespace Code.Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _shootEffect;
        [SerializeField] private GameObject _pivot;
        [SerializeField] private GameObject _model;
        [SerializeField] private float _amplitude = 0.3f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private Tween _tween;
        private Vector3 _baseLocalPos;

        private void Awake()
        {
            _baseLocalPos = _pivot.transform.localPosition;
        }

        public GameObject Pivot => _pivot;

        public void PlayShootEffect()
        {
            _shootEffect.Play();
        }

        public void Play()
        {
            Stop();

            _tween = DOTween.Sequence()
                .Append(_pivot.transform.DOLocalMoveY(_baseLocalPos.y + _amplitude, _duration).SetEase(_ease))
                .Append(_pivot.transform.DOLocalMoveY(_baseLocalPos.y - _amplitude, _duration).SetEase(_ease))
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Stop()
        {
            _tween?.Kill();
            _tween = _pivot.transform.DOLocalMoveY(_baseLocalPos.y, 0.3f).SetEase(Ease.InOutSine);
        }
    }
}