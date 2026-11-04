
# Project Eclipse — 500 Enemy Performance Baseline

## Test identity
- Task: 047
- Scene: Assets/_Game/Scenes/Test/EnemyStressTest.unity
- Branch: task/047-stress-test-500-enemies
- Unity: 6000.3.27f1
- Measurement mode: Unity Editor / Play Mode
- Test date: 2026-11-04
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
- Enemy count: 500 simultaneously active
- Enemy pooling: enabled
- Prewarm: 500
- Spawn seed: 12345
- Spawn radius: 12–18
- Dummy target orbit radius: 5
- Dummy target orbit speed: 25 degrees/s
- Player combat: disabled
- Damage popup workload: none


## Performance results

| Metric | Result | Measurement source |
|---|---:|---|
| FPS (mean of 6 observed samples) | 220 FPS | Game Stats |
| Approx. frame time derived from FPS | 4.55 ms | 1000 / 220 |
| CPU PlayerLoop (selected frame) | 1.83 ms | CPU Usage / Hierarchy |
| Physics2D (selected frame) | 0.28 ms | CPU Usage / Hierarchy |
| GC allocation (selected frame) | 0 B | Memory Profiler |
| FPS minimum observed | Not recorded | |
| Average CPU frame time | Not measured | |
| Maximum CPU frame time | Not measured | |
| Active enemy count | 500 | Runtime counts |
| Pool total count | 500 | Runtime counts |
| Registry count | 500 | Runtime counts |
| Object count growth | None observed | 60-second test |
| Console errors | 0 | Unity Console |

## Observations

- 500 BasicEnemy instances were active during the
  60-second stress test.
- The pool and registry counts remained consistent.
- Observed FPS averaged 220 across 6 samples.
- Selected-frame CPU PlayerLoop time was 1.83 ms.
- Selected-frame Physics2D time was 0.28 ms.
- Selected-frame GC allocation was 0 B.
- No continuous object count growth was observed.
- No console errors occurred.
- No performance bottleneck was confirmed.

## Conclusion

- Baseline recorded: YES
- Test stable for 60 seconds: YES
- Measurement environment: Unity Editor
- Optimization changes: NONE

### Limitations

- FPS is based on six observations, not all frames
  captured during the 60-second interval.
- CPU, Physics2D and GC metrics are selected-frame
  samples rather than 60-second averages.
- Editor profiling results are not equivalent to
  standalone Development Build performance.

### Investigation candidates for Task 049

- Compare CPU frame time at 500 enemies.
- Evaluate Physics2D scaling.
- Inspect EnemyMovement.FixedUpdate cost.
- Inspect GC allocation and frame-time spikes.

```
