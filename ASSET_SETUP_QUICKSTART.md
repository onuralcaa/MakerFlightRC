# MakerFlight RC: Free 3D Assets Integration - QUICKSTART

## ✅ What's Been Set Up

The project now has complete infrastructure for 3D models:

```
Assets/Art/Models/
├── Airplane/                      # Ready for aircraft models
├── Environment/                   # Ready for terrain/nature
├── README.md                      # Overview
└── ASSET_INTEGRATION_GUIDE.md    # Detailed instructions
```

Plus a new Editor helper: **`MakerFlight RC → Aircraft Setup`** menu

---

## 🚀 Quick Integration Path (Choose ONE Option)

### **OPTION A: Unity Asset Store (RECOMMENDED - Easiest)**

**1. Get Free Environment Pack**
- In Unity Editor: Window → Asset Store
- Search: `Low Poly Environment - Nature Free`
- Publisher: Polytope Studio
- Click "Add to My Assets" → "Import"
- Import folder: `Assets/Art/Models/Environment/`

**2. Get Free Aircraft Model from Sketchfab**
- Visit: https://sketchfab.com
- Search: `RC airplane low poly`
- Filter: License = CC0 (free, no attribution needed)
- **Recommended models:**
  - "Small Cessna Trainer"
  - "RC Airplane Simple"
  - "Low-Poly Aircraft"
- Download as **.fbx** format

**3. Import Aircraft to Project**
- Create folder: `Assets/Art/Models/Airplane/MyPlane/`
- Drag downloaded .fbx file into that folder
- Unity auto-imports

---

### **OPTION B: Quaternius (Also Free)**

**For Airplane:**
- Visit: https://quaternius.com
- Download: Any aircraft model as .blend
- Open in Blender, Export as .fbx to `Assets/Art/Models/Airplane/`

**For Environment:**
- Same source: Download terrain/tree packs
- Save to `Assets/Art/Models/Environment/`

---

### **OPTION C: Mix & Match**
- Aircraft from Sketchfab
- Terrain from Quaternius
- Vegetation from Unity Asset Store free pack

---

## 📝 Step-by-Step Scene Integration

### **Step 1: Delete Old Models**

In Unity Hierarchy (Simulation_Bootstrap scene):
1. Find `[AIRCRAFT_SPAWNER]` → `AircraftController` (gray capsule)
2. Right-click → Delete
3. Find `[ENVIRONMENT]` → `Plane` (white runway)
4. Right-click → Delete

### **Step 2: Create New Aircraft GameObject**

1. Right-click `[AIRCRAFT_SPAWNER]` in Hierarchy
2. Create Empty → Name: `AircraftController`
3. Set Position to (0, 1, 0)

### **Step 3: Add Aircraft Model**

1. In Project: Navigate to `Assets/Art/Models/Airplane/[Your Model]/[model.fbx]`
2. Drag the .fbx into the `AircraftController` GameObject (makes it a child)
3. The model should appear in scene
4. If model is tiny/huge, adjust child's Scale:
   - Small model → Scale (2, 2, 2)
   - Large model → Scale (0.5, 0.5, 0.5)

### **Step 4: Wire Aircraft Controller**

1. Select `AircraftController` (parent) in Hierarchy
2. In Inspector, Add Component → Search "Rigidbody"
3. Set:
   - Mass: 1.2
   - Use Gravity: ✓ (checked)

4. Add Component → Search "KeyboardInputProvider" (select from MakerFlightRC namespace)

5. Add Component → Search "AircraftController" (select from MakerFlightRC.Runtime.Aircraft)

6. **Wire Inspector Fields:**
   - Drag `Assets/Data/Default_Aircraft.asset` → `defaultAircraft`
   - Drag `Assets/Channels/InputChannel.asset` → `inputChannel`
   - Drag `Assets/Channels/FlightDataChannel.asset` → `flightDataChannel`

**OR use Helper:**
1. Select `AircraftController`
2. Menu: `MakerFlight RC → Aircraft Setup → Wire Selected Aircraft`
3. Auto-wires all components ✓

### **Step 5: Add Collider**

1. Select `AircraftController` (parent, not model child)
2. Menu: `MakerFlight RC → Aircraft Setup → Add Capsule Collider`
3. Collider auto-configured

### **Step 6: Add Terrain/Environment**

1. Right-click `[ENVIRONMENT]` in Hierarchy
2. Drag terrain meshes from `Assets/Art/Models/Environment/` into it
3. Position terrain at Y = -0.5 to be below aircraft
4. Scale terrain as needed (try 5x for runway)

### **Step 7: Add Terrain Collider**

1. Select terrain GameObject
2. Add Component → Physics → Box Collider (for flat runway)
3. OR → Mesh Collider (for complex terrain, uncheck "Convex")

### **Step 8: Test!**

1. Press Play
2. Verify:
   - ✓ Aircraft visible
   - ✓ Terrain visible
   - ✓ Camera follows aircraft from rear
   - ✓ No blue screen
   - ✓ Press W → aircraft accelerates
   - ✓ Arrow keys control pitch/yaw

---

## 🔧 Helper Editor Commands

After setup, these menu items make future setup easier:

```
MakerFlight RC → Aircraft Setup
├── Wire Selected Aircraft       # Auto-wire all components
├── Add Capsule Collider        # Quick collider setup
└── Add Box Collider            # For flat surfaces
```

---

## 📚 Documentation

Full detailed guides included:

- **`Assets/Art/Models/README.md`** - Overview & folder structure
- **`Assets/Art/Models/ASSET_INTEGRATION_GUIDE.md`** - 30+ step walkthrough

---

## ✨ Expected Result After Integration

**Scene should show:**
- Aircraft model on runway (not capsule)
- Visible terrain/trees/environment
- Camera following from rear position
- Responsive controls (W-A-S-D, arrows)
- Realistic runway landing surface

**Example play:**
1. Press Play
2. Press W key → aircraft accelerates forward
3. Arrow Up/Down → pitch up/down
4. Arrow Left/Right → turn
5. Aircraft flies and lands on terrain

---

## ⚠️ Troubleshooting Checklist

| Problem | Fix |
|---------|-----|
| Model invisible | Check Scale (might be 0.001), try Scale (1,1,1) |
| Aircraft falls through terrain | Add Collider to terrain |
| Model rotated wrong | Select parent, rotate Y by 180° |
| Black/purple model | Check Materials imported in .fbx, assign default mat |
| Weird physics | Verify Rigidbody on parent (not child) |
| No input response | Check InputChannel reference is assigned |
| Blue screen | Aircraft position went NaN - check colliders on terrain |

---

## 🎯 Next Steps (After Integration)

1. ✅ **Download free models** (Sketchfab + Asset Store)
2. ✅ **Import to Assets/Art/Models/**
3. ✅ **Replace scene models** (follow steps above)
4. ✅ **Test in Play mode**
5. 🔜 **Build standalone .exe** (next phase)
6. 🔜 **Add UI layers** (Main Menu, Garage, HUD - future)

---

## 📖 Recommended Models (Tested Low-Poly)

### Aircraft
- **"Cessna 172" by Quaternius** (quaternius.com)
- **"Small RC Plane" on Sketchfab** (search & filter CC0)

### Environment
- **"Low Poly Environment Nature Free"** - Unity Asset Store (FREE)
- **"Terrain & Trees" by Quaternius** (quaternius.com)

---

**Last Updated:** May 25, 2026
**Framework:** Unity 2022.3 LTS + New Input System

---

## Questions?

Refer to:
1. `Assets/Art/Models/ASSET_INTEGRATION_GUIDE.md` - Detailed walkthrough
2. `Assets/Art/Models/README.md` - Asset overview
3. GitHub Issues - For bugs or questions

---

**Status:** ✅ Infrastructure Ready | ⏳ Waiting for Model Downloads
