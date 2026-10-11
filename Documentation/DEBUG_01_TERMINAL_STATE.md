# Debug iteration 1

Guard an inactive/dead player before the clock advances, reject late projectile requests and disable Entities processing after a result. Existing actors and bullets freeze; an already final result cannot be overwritten.

The intermediate branch isolates terminal-state hardening for review. The original verification record covered no ten-kill victory, no 60-second spawn, prior-death defeat, result stability and restart.
