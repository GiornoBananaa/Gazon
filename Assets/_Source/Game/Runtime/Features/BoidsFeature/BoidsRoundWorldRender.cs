using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Runtime.BoidsSystem;
using Game.Runtime.CameraSystem;
using Game.Runtime.Utils;
using Reflex.Attributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Runtime.BoidsFeature
{
    public class BoidsRoundWorldRender : Boids.BoidsComponent
    {
        private ICurrentCamera _camera;
        private CancellationTokenSource _cancellationTokenSource;
        
        [Inject]
        public void Construct(ICurrentCamera currentCamera)
        {
            _camera = currentCamera;
        }

        protected override void OnBoidsGenerated()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _ = UpdateRenderBounds(_cancellationTokenSource.Token);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        private void Update()
        {
            var cameraPosition = _camera.GetCurrentCamera().transform.position;
            transform.position = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z);
        }

        private async UniTaskVoid UpdateRenderBounds(CancellationToken token)
        {
            const float maxTimeOfRenderUpdate = 0.1f;
            const int targetFps = 60;
            
            await UniTask.WaitUntil(() => Boids.Entities != null, cancellationToken: token);
            
            int framesOfUpdate = math.max(1, (int)(maxTimeOfRenderUpdate * targetFps));
            int batchSize = Boids.Entities.Count / framesOfUpdate;
            float nextBatchTime = maxTimeOfRenderUpdate / framesOfUpdate;
            
            int batchIndex = 0;
            
            while (gameObject != null && Boids.Entities != null && !token.IsCancellationRequested)
            {
                await UniTask.WaitForSeconds(nextBatchTime, cancellationToken: token);
                
                var cameraPosition = _camera.GetCurrentCamera().transform.position;
                int start = batchIndex * batchSize;
                int end = batchIndex == framesOfUpdate - 1 ? Boids.Entities.Count : start + batchSize;
                
                for (int i = start; i < end; i++)
                {
                    var entity = Boids.Entities[i];
                    entity.Renderer.localBounds = new Bounds(
                        entity.transform.InverseTransformPoint(MathUtils.ConvertToRoundWorldPosition(entity.Renderer.transform.position + entity.Filter.mesh.bounds.center, cameraPosition, 0.006f)),
                        entity.Filter.mesh.bounds.size * 3);
                }

                batchIndex++;
                if (batchIndex >= framesOfUpdate) batchIndex = 0;
            }
        }
    }
}