# Debug iteration 2

Symptom: a kill near a ten-second boundary gave the impression that survivors disappeared and were replaced by a larger wave. Code inspection showed that only the killed enemy freed itself. The clock reset _nextSpawn at each phase boundary, causing an abrupt immediate batch.

Remove that reset, _lastPhase and the per-phase _phaseSpawnCount reset. Retain the already scheduled spawn and use the current phase interval after that natural spawn. Survivors retain their instance, parent, HP and ongoing behaviour.

The saved Godot 4.7 .NET integration run passed 344 assertions, including the added scheduling and survivor-instance/HP checks. Final gameplay source equals the supplied fixed project after line-ending normalisation.
