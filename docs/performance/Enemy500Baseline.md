
# Project Eclipse — 500 Enemy Performance Baseline

## Test identity
- Task: 048
- Scene: Assets/_Game/Scenes/Test/EnemyStressTest.unity
- Branch: task/048-stress-test-500-enemies
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
| FPS (mean of 6 observed samples) | 100 FPS | Game Stats |
| Approx. frame time derived from FPS | 10.00 ms | 1000 / 100 |
| CPU PlayerLoop (selected frame) | 36.52 ms | CPU Usage / Hierarchy |
| Physics2D (selected frame) | 32.17 ms | CPU Usage / Hierarchy |
| GC allocation (selected frame) | 0 B | Memory Profiler |
| FPS minimum observed | Not recorded | |
| Average CPU frame time | Not measured | |
| Maximum CPU frame time | Not measured | |
| Active enemy count | 500 | Runtime counts |
| Pool total count | 500 | Runtime counts |
| Registry count | 500 | Runtime counts |
| Object count growth | None observed | 60-second test |
| Console errors | 0 | Unity Console |

## Comparison with Task 047

| Metric | 250 enemies | 500 enemies |
|---|---:|---:|
| Observed FPS (6-sample mean) | 220 | 100 |
| Approx. frame time from FPS | 4.55 ms | 10.00 ms |
| PlayerLoop (selected frame) | 1.83 ms | 36.52 ms |
| Physics2D (selected frame) | 0.28 ms | 32.17 ms |
| GC Alloc (selected frame) | 0 B | 0 B |

## Observations

- 500 BasicEnemy instances were active during the
  60-second stress test.
- Pool and registry counts remained stable at 500.
- Observed FPS decreased from 220 to 100 when
  enemy count increased from 250 to 500.
- A selected frame showed PlayerLoop = 36.52 ms
  and Physics2D = 32.17 ms.
- Physics2D is a candidate performance hotspot.
- The CPU/Physics2D measurements are single-frame
  samples and may represent a spike.
- Selected-frame GC allocation was 0 B.
- No continuous object count growth was observed.
- No console errors occurred.

## Conclusion

- Baseline recorded: YES
- Test stable for 60 seconds: YES
- Measurement environment: Unity Editor
- Optimization changes: NONE

### Potential bottleneck

Physics2D is the leading investigation candidate
based on the selected-frame sample.

If both measurements belong to the same frame and
main thread, Physics2D accounts for approximately
88.1% of the sampled PlayerLoop time.

This does not establish a persistent Physics2D
bottleneck. Repeated-frame profiling is required.

### Limitations

- FPS is based on six observations, not all frames
  captured during the 60-second interval.
- CPU, Physics2D and GC metrics are selected-frame
  samples rather than 60-second averages.
- The selected 36.52 ms frame is not representative
  of the observed 100 FPS mean without further data.
- Editor profiling is not equivalent to a
  standalone Development Build.

### Investigation candidates for Task 049

1. Inspect Physics2D costs across multiple frames.
2. Examine collision contacts as enemies cluster.
3. Inspect EnemyMovement.FixedUpdate.
4. Compare normal frames with CPU spikes.
5. Measure before/after optimization using the
   same 500-enemy workload.

```
