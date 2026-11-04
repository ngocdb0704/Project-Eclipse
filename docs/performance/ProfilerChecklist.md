
# Project Eclipse — Unity Profiler Checklist

## 1. Purpose

This document defines a repeatable performance
benchmark procedure for Project Eclipse.

Goals:
- Detect FPS and frame-time regressions.
- Monitor CPU and Physics2D workload.
- Detect managed memory allocations.
- Monitor rendering cost and object counts.
- Compare results using consistent conditions.
- Preserve evidence for future optimization.

This checklist does not authorize gameplay or
physics behavior changes without separate review.

## 2. Benchmark Environment

Record this information for every benchmark.

| Field | Value |
|---|---|
| Unity version | 6000.3.27f1 |
| Test date | |
| Git commit SHA | |
| Scene | |
| Test mode | Editor / Development Build |
| Operating system | |
| CPU | |
| GPU | |
| RAM | |
| Game resolution | |
| VSync | |
| Target frame rate | |
| Quality settings | |
| Deep Profile | OFF |

Unknown values must be marked "Not recorded".
Do not invent hardware information.

Editor measurements are approximate and may
include Editor-related overhead.

Use a standalone Development Build for stronger
release-performance conclusions.

## 3. Standard Benchmark Workload

### Stress test scene

Assets/_Game/Scenes/Test/EnemyStressTest.unity

### Standard configuration

| Setting | Value |
|---|---|
| Enemy prefab | BasicEnemy |
| Active enemies | 500 |
| Pool prewarm | 500 |
| Random seed | 12345 |
| Spawn radius | 12–18 |
| Dummy target moving | YES |
| Target orbit radius | 5 |
| Orbit speed | 25 degrees/second |
| Warmup | 10 seconds |
| Observation window | 60 seconds |

For comparison with Task 047, the earlier
250-enemy baseline is available in:
Enemy250Baseline.md

The Task 048 baseline is available in:
Enemy500Baseline.md

Do not save changes to the stress scene as
part of Task 050.

## 4. Unity Profiler Setup

1. Open the EnemyStressTest scene.
2. Open Window > Analysis > Profiler.
3. Select the Play Mode profiling target.
4. Disable Deep Profile.
5. Enable CPU Usage, Memory, Physics 2D
   and Rendering modules when available.
6. Start Play Mode.
7. Verify 500 active enemies.
8. Allow 10 seconds for warmup.
9. Observe performance for 60 seconds.
10. Inspect normal frames and visible spikes.
11. Record the results in a benchmark report.

The Profiler may retain only a limited recent
frame history. Observing 60 seconds does not
automatically produce a 60-second average.

If using sample observations, record their count,
sampling method and limitations.

## 5. Metrics Checklist

### FPS and Frame Time

- [ ] Record observed FPS.
- [ ] Record FPS sampling method.
- [ ] Record CPU frame time in milliseconds.
- [ ] Record a representative frame.
- [ ] Inspect unusually slow frames.
- [ ] Record average/P95 only when actually measured.
- [ ] Identify VSync or FPS cap conditions.

For a 60 FPS target, the total frame budget is
approximately 16.67 ms.

PlayerLoop time is not identical to total
frame time or CPU active time.

### CPU Usage

- [ ] Open CPU Usage > Hierarchy.
- [ ] Inspect Main Thread.
- [ ] Record PlayerLoop time.
- [ ] Inspect FixedUpdate-related samples.
- [ ] Look for expensive script methods.
- [ ] Check Calls and Time ms columns.
- [ ] Inspect spike frames separately.

### Garbage Collection and Memory

- [ ] Record GC Allocated In Frame.
- [ ] Inspect several steady-state frames.
- [ ] Inspect frames with GC spikes.
- [ ] Check for increasing managed memory.
- [ ] Record whether GC collection occurred.
- [ ] Distinguish one-frame results from averages.

### Physics 2D

- [ ] Record Physics2D simulation time.
- [ ] Inspect normal and spike frames.
- [ ] Check physics simulation call counts.
- [ ] Record contact counts when available.
- [ ] Record collision callbacks when available.
- [ ] Check Rigidbody2D activity.
- [ ] Identify unexpectedly high physics cost.

Do not infer persistent bottlenecks from
one selected frame.

### Rendering

- [ ] Open Rendering Profiler module.
- [ ] Record draw calls if available.
- [ ] Record batches.
- [ ] Record SetPass calls.
- [ ] Record triangles/vertices if available.
- [ ] Check rendering spikes.
- [ ] Record Game view resolution.

Draw calls, batches and SetPass calls are
different metrics. Do not treat them as
interchangeable.

### Object Counts and Pooling

- [ ] Verify Active enemy count = 500.
- [ ] Verify Registry count = 500.
- [ ] Verify Pool CountAll = 500.
- [ ] Check for unexpected instantiated objects.
- [ ] Verify object counts remain stable.
- [ ] Confirm no continuous pool growth.
- [ ] Confirm Unity Console has zero errors.

## 6. Initial Warning Thresholds

These thresholds are preliminary investigation
triggers, not strict release acceptance criteria.

| Metric | Initial warning condition |
|---|---|
| FPS | Sustained below 60 FPS |
| Total frame time | Repeatedly above 16.67 ms |
| CPU frame time | Repeatedly above the frame budget |
| GC Alloc | Repeated steady-state allocations |
| Physics2D | Repeated high cost or spikes |
| Draw calls / Batches | Large unexplained increase |
| Object count | Growth beyond expected pool size |
| Console errors | Any unexpected error |

Rendering warnings require a comparable
baseline from the same scene and environment.

Physics2D has no fixed numeric release
threshold yet. Record repeated-frame results
before establishing one.

A single CPU or Physics2D spike requires
investigation, not automatic failure.

## 7. Existing Baseline Reference

### Task 047 — 250 enemies

- FPS: 220 (mean of 6 observations).
- PlayerLoop: 1.83 ms (selected frame).
- Physics2D: 0.28 ms (selected frame).
- GC Alloc: 0 B (selected frame).
- Pool CountAll: 250.

### Task 048 — 500 enemies

- FPS: 100 (mean of 6 observations).
- PlayerLoop: 36.52 ms (selected frame).
- Physics2D: 32.17 ms (selected frame).
- GC Alloc: 0 B (selected frame).
- Pool CountAll: 500.

The CPU and Physics2D readings are not
60-second averages.

The high Physics2D sample at 500 enemies is
a performance investigation candidate.

Task 049 optimization was deferred.
No verified before/after improvement is claimed.

## 8. Reporting Template

### Test Information

- Date:
- Git commit:
- Scene:
- Enemy count:
- Editor / Development Build:
- Hardware:
- Resolution:
- Warmup duration:
- Observation duration:

### Results

| Metric | Result | Measurement method |
|---|---|---|
| FPS | | |
| Frame time | | |
| PlayerLoop | | |
| Physics2D | | |
| GC Alloc/frame | | |
| Draw calls | | |
| Batches | | |
| SetPass calls | | |
| Active objects | | |
| Pool CountAll | | |
| Console errors | | |

### Findings

- Main performance concern:
- Normal frame observations:
- Spike observations:
- Repeated allocation:
- Unexpected object growth:
- Limitations:

### Decision

- Benchmark reproducible:
- Regression detected:
- Further investigation required:
- Approved by:

## 9. Definition of Done

- [ ] Profiler configuration is documented.
- [ ] Workload is reproducible.
- [ ] FPS, CPU and GC metrics are covered.
- [ ] Physics2D metrics are covered.
- [ ] Rendering metrics are covered.
- [ ] Object and pool counts are covered.
- [ ] Initial warning thresholds are defined.
- [ ] Hardware and test environment are documented.
- [ ] Editor vs Build limitations are explained.
- [ ] One benchmark pass has been completed.
- [ ] README links to this checklist.

## 10. References

- Enemy250Baseline.md
- Enemy500Baseline.md
- Unity Manual: Profiler overview
  https://docs.unity3d.com/Manual/Profiler.html
