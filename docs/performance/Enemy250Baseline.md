
# Project Eclipse — 250 Enemy Performance Baseline

## Test identity
- Task: 047
- Scene: Assets/_Game/Scenes/Test/EnemyStressTest.unity
- Branch: task/047-stress-test-250-enemies
- Unity: 6000.3.27f1
- Measurement mode: [Editor / Development Build]
- Test date:
- Test duration: 60 seconds
- Warmup: 10 seconds

## Environment
- CPU:
- GPU:
- RAM:
- Operating system:
- Resolution:
- VSync:
- Target frame rate:
- Profiler Deep Profile: OFF

## Workload
- Enemy type: BasicEnemy
- Enemy count: 250 simultaneously active
- Enemy pooling: enabled
- Prewarm: 250
- Spawn seed: 12345
- Spawn radius: 12–18
- Dummy target orbit radius: 5
- Dummy target orbit speed: 25 degrees/s
- Player combat: disabled
- Damage popup workload: none

## Performance results
| Metric | Result | Measurement source |
|---|---:|---|
| FPS average | TBD | |
| FPS minimum observed | TBD | |
| CPU frame time average (ms) | TBD | |
| CPU frame time maximum (ms) | TBD | |
| Main thread time (ms) | TBD | |
| GC Alloc/frame (B) | TBD | |
| Physics2D time (ms) | TBD | |
| Active enemy count | 250 | Runtime counts |
| Pool total count | 250 | Runtime counts |
| Console errors | 0 expected | Console |

## Observations
- Main bottleneck:
- CPU spikes:
- GC spikes:
- Object count growth:
- Visual/physics issues:

## Conclusion
- Baseline recorded: [YES/NO]
- Test stable for 60 seconds: [YES/NO]
- Investigation candidates for Task 049:
```
