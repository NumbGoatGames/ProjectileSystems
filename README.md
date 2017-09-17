# Firing projectile in Unity
This is a project that shows one approach to firing physics based projectiles at targets,
both moving and static.

# Unity Version
This project was built using Unity 2017.1.0p4, but should work fine with most other versions

Windows: https://beta.unity3d.com/download/892c0f8d8f8a/UnityDownloadAssistant-2017.1.0p4.exe

macOs: https://beta.unity3d.com/download/892c0f8d8f8a/UnityDownloadAssistant-2017.1.0p4.dmg

# Usage
Copy the NumbGoat/ProjectileSystems folder to your unity project.  
Read the methods in TrajectoryHelper to get started.  

See [NumbGoat/Scripts/ProjectileTestFire.cs](Assets/NumbGoat/Scripts/ProjectileTestFire.cs) for examples.  

# Contributors

### GitHub
#### Sarah Dobie (dobiewan)
Peer reviewing code, branch maintainence, sanity checker.

#### Timothy Gray (TAGray)
Copying scripts from various locations, creating test scene, testing the scripts.

#### Steven Morrison (ContagionNZ)
Projectile models, other visual assets.

#### vengefulmollusc
Provided first draft of the code.

#### benloong
https://gist.github.com/benloong/4661336

### External
#### JamesLeeNZ
https://forum.unity3d.com/threads/projectile-trajectory-accounting-for-gravity-velocity-mass-distance.425560/#post-2750631



# Known issues
* Y of projectile does not work correctly, for slow speeds and target above shooter.

# Change log
```
v1.0.1
  - Refactored to allow easy copying of code to another project.
  - Added IHittable
v1.0.0
  - Working code
  - Test scene with projectiles hitting a number of test targets.
```

# Licence
Apache License 2.0
See LICENCE file.