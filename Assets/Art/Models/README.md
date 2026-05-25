# MakerFlight RC - 3D Art Assets

This folder contains all 3D models for the MakerFlight RC flight simulator.

## Folder Structure

```
Assets/Art/Models/
├── Airplane/                  # Aircraft models and variants
│   ├── Trainer_Cessna/       # Example: RC Trainer airplane
│   ├── SimpleRC/             # Example: Alternative aircraft
│   └── [More aircraft...]
├── Environment/               # Terrain, runway, props
│   ├── Terrain/              # Ground/landscape
│   ├── Runway/               # Takeoff/landing surface
│   ├── Vegetation/           # Trees, bushes
│   └── [More props...]
├── Materials/                 # Shared materials
├── ASSET_INTEGRATION_GUIDE.md # Detailed integration instructions
└── README.md                  # This file
```

## Getting Free Models

### Quick Start (Easiest)

**Unity Asset Store - Free Pack:**
1. Open Unity Editor
2. Window → Asset Store
3. Search: "Low Poly Environment - Nature Free"
4. Author: Polytope Studio
5. Click "Add to My Assets"
6. Click "Import"
7. Select folder: `Assets/Art/Models/Environment/`

### Alternative Sources

- **Sketchfab.com** - Search "RC airplane low poly", filter by CC0/free license
- **Quaternius.com** - Specializes in low-poly free models
- **TurboSquid Free** - turbosquid.com/Search/3D-Models/free
- **CGTrader Free** - Selected free models on cgtrader.com
- **Poly Haven** - polyhaven.com (high-quality free models)

## Current Status

- **Airplane Model:** Not imported yet (see ASSET_INTEGRATION_GUIDE.md)
- **Environment:** Not imported yet (see ASSET_INTEGRATION_GUIDE.md)

## How to Add Models

### Step 1: Download
Get a free model from one of the sources above in .fbx or .obj format.

### Step 2: Import to Unity
Create appropriate subfolder and drag .fbx into `Assets/Art/Models/`.

### Step 3: Configure in Scene
Use menu: **MakerFlight RC → Aircraft Setup → Wire Selected Aircraft** (for aircraft models)

### Step 4: Test
Press Play and verify model renders correctly.

## File Format Guidelines

- **Format:** .fbx (preferred) or .obj
- **Polycount:** < 50,000 triangles for best performance
- **Textures:** 1024x1024 or smaller
- **Scale:** Export at real-world scale (1 unit = 1 meter)

## Recommended Aircraft Models

### For RC Trainer Style
- Search Sketchfab: "small airplane" + "low poly" + filter CC0
- Look for Cessna 172, Piper Cub, or generic trainer aircraft
- Target: 5,000-20,000 triangles

### For RC Racing Drone Style
- Search: "quadcopter" or "fpv drone" low poly
- Target: 3,000-10,000 triangles

## Recommended Environment Assets

### Terrain/Runway
- Flat plane runway: 50x200 units
- Grass field: surrounding terrain with simple trees
- Modular pieces can be combined

### Vegetation
- Low-poly trees (1,000-3,000 triangles each)
- Simple bushes and shrubs
- Rocks and small rocks for detail

## License Considerations

When using free models, ensure compliance with licenses:

- **CC0** - Free, no attribution required
- **CC-BY** - Free, attribution required
- **CC-BY-SA** - Free, attribution and share-alike required
- Always check model's license page

## Integration Helpers

**Editor Menu Commands:**
- `MakerFlight RC → Aircraft Setup → Wire Selected Aircraft` - Automatically wire all components
- `MakerFlight RC → Aircraft Setup → Add Capsule Collider` - Add collision shape
- `MakerFlight RC → Aircraft Setup → Add Box Collider` - Add collision shape

See [ASSET_INTEGRATION_GUIDE.md](ASSET_INTEGRATION_GUIDE.md) for detailed step-by-step instructions.

## Performance Optimization

1. **LOD Groups:** Use if imported model includes Level of Detail
2. **Material Batching:** Combine meshes where possible
3. **Collider Optimization:** Use simple colliders (Box, Capsule) over Mesh Collider
4. **Texture Atlasing:** Combine textures to reduce draw calls

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Model too small/large | Adjust Scale in Transform |
| Model upside down | Rotate on X or Y axis by 180° |
| Model falls through terrain | Add Collider to terrain |
| Weird model orientation | Forward should be Z-axis |
| Plane doesn't render | Check model's LOD visibility, material assignment |

---

**Last Updated:** May 25, 2026
**Format Version:** 1.0
