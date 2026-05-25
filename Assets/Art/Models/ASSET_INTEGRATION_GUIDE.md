# MakerFlight RC - Asset Integration Guide

## Overview
This guide explains how to integrate free low-poly 3D models into the MakerFlight RC project.

## Folder Structure
```
Assets/Art/Models/
├── Airplane/           # RC trainer airplane models
├── Environment/        # Terrain, runway, vegetation
└── ASSET_INTEGRATION_GUIDE.md
```

## Recommended Free Assets

### 1. RC Airplane Models
**Option A: Sketchfab (Recommended)**
- Search: "RC airplane low poly" or "small aircraft free"
- Filter: Downloadable, CC0/CC-BY license
- Popular models:
  - Small Cessna-style trainer aircraft
  - Propeller-driven RC planes
  - Recommended format: .fbx or .blend (convert to .fbx)

**Option B: Quaternius (Low-Poly specialists)**
- Website: https://quaternius.com/
- Free pack: "Vehicles Low Poly Pack"
- Format: .blend (requires Blender for export to .fbx)

**Option C: TurboSquid Free**
- Website: https://www.turbosquid.com/Search/3D-Models/free
- Search: "RC airplane" or "small aircraft"

### 2. Terrain & Environment
**Recommended: Low Poly Environment - Nature Free (Unity Asset Store - FREE)**
- Publisher: Polytope Studio
- Link: https://assetstore.unity.com/packages/187052
- Contains: Trees, grass, terrain, building blocks
- Perfect for low-poly environments
- Installation: Window → TextureEditor → Asset Store → Search → Download & Import

**Alternative: Quaternius Terrain & Nature**
- Free low-poly terrain sets
- Modular components

## Import Steps

### Step 1: Download Model Files
1. Choose and download your selected models
2. Ensure format is .fbx or .blend (can be converted in Blender)
3. Save to temporary folder

### Step 2: Import to Unity

#### For FBX Models:
1. Create subfolder in `Assets/Art/Models/Airplane/` for the model
   - Example: `Assets/Art/Models/Airplane/Trainer_Cessna/`
2. Drag & drop .fbx file into that folder
3. Unity auto-imports with model settings
4. Check Import Settings in Inspector:
   - Model tab: ✓ Skinned Mesh Renderer (if rigged)
   - Materials tab: ✓ Import Materials
   - Animations tab: ✓ Import Animation (if present)

#### For Asset Store Packages:
1. In Unity Editor: Window → Asset Store
2. Search for "Low Poly Environment"
3. Click "Add to My Assets"
4. Click "Import"
5. Select location: `Assets/Art/Models/Environment/`
6. Click "Import"

### Step 3: Prepare Model for Scene

**For Airplane Model:**
1. In Project folder, find the imported model prefab/mesh
2. Drag into scene to test
3. Adjust rotation/scale if needed:
   - Typical scale: (0.5, 0.5, 0.5) to (2, 2, 2)
   - Rotation: (0, 0, 0) for upright aircraft
4. Delete test instance (don't save)

**For Terrain/Environment:**
1. Drag terrain pieces into scene
2. Arrange runway, trees, hills
3. Keep at Y=0 or Y=-0.5 to be below aircraft starting position
4. Scale as needed to create landing strip

### Step 4: Update Scene Hierarchy

**Current hierarchy to replace:**
```
Simulation_Bootstrap
├── Main Camera
├── [ENVIRONMENT]
│   └── Plane (DELETE THIS)
└── [AIRCRAFT_SPAWNER]
    └── AircraftController (Capsule - REPLACE)
```

**New hierarchy:**
```
Simulation_Bootstrap
├── Main Camera
├── [ENVIRONMENT]
│   ├── Terrain (imported terrain pieces)
│   ├── Runway (imported runway mesh)
│   └── [other vegetation as needed]
└── [AIRCRAFT_SPAWNER]
    └── AircraftController (new aircraft model with Rigidbody + scripts)
```

## Configuration Steps

### Replace Airplane Model in Scene

1. **Delete old Capsule:**
   - In Hierarchy, right-click `AircraftController` GameObject
   - Select "Delete"

2. **Create new Aircraft GameObject:**
   - Right-click `[AIRCRAFT_SPAWNER]`
   - Create empty GameObject, name: `AircraftController`
   - Position: (0, 1, 0)
   - Scale: (1, 1, 1)

3. **Import model as child:**
   - Drag your imported .fbx model into the new `AircraftController`
   - The model becomes a child of AircraftController
   - In Inspector, adjust child's Position to (0, 0, 0) and Scale if needed

4. **Add Rigidbody:**
   - Select `AircraftController` (parent)
   - Add Component → Physics → Rigidbody
   - Mass: 1.2
   - Use Gravity: ✓
   - Constraints: Leave empty
   - Collision: Add Box Collider or Capsule Collider (match model size)

5. **Add Scripts:**
   - Select `AircraftController`
   - Add Component → KeyboardInputProvider
   - Add Component → AircraftController (Main script)

6. **Wire Inspector References:**
   - Drag `/Assets/Data/Default_Aircraft.asset` → defaultAircraft field
   - Drag `/Assets/Channels/InputChannel.asset` → inputChannel field
   - Drag `/Assets/Channels/FlightDataChannel.asset` → flightDataChannel field

7. **Remove old Colliders (if imported model has them):**
   - Select model child GameObject
   - Remove any Collider components (we use parent's collider)

### Update Terrain in Scene

1. **Delete old Plane:**
   - In Hierarchy, find `Plane` under `[ENVIRONMENT]`
   - Right-click → Delete

2. **Import new terrain:**
   - Drag terrain meshes from Assets/Art/Models/Environment/
   - Adjust positions to create runway and landing area
   - Keep Y around -0.5 or 0
   - Scale as needed (typical: 2-5x for runway)

3. **Add Colliders to Terrain:**
   - Select terrain GameObject
   - Add Component → Collider
   - For flat runway: Box Collider (width=20, height=1, depth=100)
   - For complex terrain: Mesh Collider (convex: unchecked)

## Testing After Integration

1. **Enter Play Mode:** Click Play button
2. **Expected Results:**
   - Camera follows new airplane model from rear
   - Airplane visible on runway/terrain
   - No blue screen
   - Airplane doesn't teleport to infinite coordinates
   - Input responsive (press W for throttle, arrows for pitch/yaw)
   - Airplane accelerates with thrust

3. **If Issues:**
   - Check Console for errors
   - Verify all Inspector fields are filled (no null references)
   - Confirm colliders are properly assigned
   - Check model scale isn't too large/small

## Model Optimization Tips

1. **Polycount:** Keep < 50,000 polygons for smooth performance
2. **Textures:** 1024x1024 or smaller for textures
3. **LOD Groups:** If model has Level of Detail, configure for distance culling
4. **Materials:** Reduce number of unique materials for better batching

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Airplane invisible | Check model scale (might be too small), adjust Position/Scale |
| Airplane falls through terrain | Verify Colliders are present and overlapping with terrain |
| Weird rotation | Rotate parent AircraftController GameObject 90° on Y-axis |
| No collision response | Ensure Rigidbody is on parent, not on model child |
| Model appears inside-out | Check model's normals in original software, or flip mesh in Unity |

## Notes

- All models should use Z-axis pointing forward for correct aircraft orientation
- Keep center of mass near origin for stable flight physics
- Test with simple cube first if uncertain about model orientation

---
**Last Updated:** May 25, 2026
