# Examples

In order to use these example files, just open the associated RFEM file and then explore and run the grasshopper script.

## Get Data

The process of importing data from RFEM into Grasshopper revolves around the *"Get Data" Component*. The following concepts are explored:

- Functioning of "Get Data" Component
- Object Oriented structure of the RFEM Plugin
- Filters
- Supported geometry types - pretty much everything
- Supported List formats
- Serialization of RF-Objects
- Working with several open RFEM models
- Panels as a way of displaying object properties
- Casting of RF-Objects into Grasshopper Geometry Types
- Definition of nodal supports with custom orientation

![alt text](https://h29vva.db.files.1drv.com/y4m2mtz9GQfnzjrJM7WsoqjKn-bQ3o0XcVv6-FTBDFqFHaNjhOuyguZ4HW8FZZT3gJfWfesNasBKN8tprd425d_-KcJJi9N97Rvd-4XRrhlybcEydRgkF1Ofs8lkBL2WUkrSUR6pzk2RNoOwrurjgTKNtid6YPJ_F8U_zkNPsyakkrT6nR3KOEGoPER8GCzVDSy2upRu682712h7BEVG_VL3Q?width=972&height=547&cropmode=none "Get Data - GH")
![alt text](https://ig9vva.db.files.1drv.com/y4m9N7MTo8EYkfIv5ZBXOsCtkNv9O-KqC5ru1wEU0Mi5G1VY_cPAqmGdkqJ1zlEA35qmFlND7-S93cE8kRPgKsPLDpvCIEApDj0u0qdtrFDrO9OuufnX57lzXtxnM1u9-3_RxkqqeoWetj3O5GDEFZeWn-tZyCpONMK2JfyL3n5qwRTRgrQuqHjn86IWSXbscDKrVYR1OhSGhJA4dRw2HkPNA?width=862&height=480&cropmode=none "Get Data - REFM")

## Set Data

The process of exporting data from Grasshopper into RFEM revolves around the *"Set Data" Component*. The following concepts are explored:

- Functioning of "Set Data" Component
- How to bake RF-Objects in RFEM
- How to modify existing RFEM Objects.
- How to create a Live-Link between Grasshopper and RFEM
- Enumerations in RFEM (LineType, MemberType, SurfaceGeometryType...)
- Versatility of Data Structures in the RFEM Plugin

![alt text](https://m4jmjq.db.files.1drv.com/y4mmAejTyXGwkNPGK45dnLednLBa8Si6Lsil9cGLeFvK_eu98O2Bnv3FBA3RfRcDcXGhzJsVT5sXWUI0Iqr8kKp7p2MeZsqZ9rwwOO2KmCRxjR-wrsS7WPP4dMysCaUS84IhMnbGG2XSgNgaQtAJLh4Wf8T1MuT7pxNHYZ-bk1Q46CNQ7aCYzSbzBhZbWbVOTSaRg8Suotk-wA-3AbazYaSLg?width=773&height=389&cropmode=none "Set Data - GH")
![alt text](https://mojmjq.db.files.1drv.com/y4m9c61AZgWTYQnu0oUWVTVLjzODxq2ZFVZ5X6JQWtD3TGBY0RiP9s_tD7MefUE_fFHoqxd0JCCpppdmwg0_LOFOI_0hmiXLImeibVIG-KqrBRn6PPYhKL3XLKAetnojveAwJg_H_1Rg830EAqUKhVlQy8_6z_KlqtthizYBC0BNci7JI8FgPQ1P_XzLTdjYnyRkw4zGjmjQ0RQiZq3IdsnsA?width=843&height=494&cropmode=none "Set Data - REFM")

## Concrete Building

Learn how to export a whole model from Grasshopper to RFEM

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Concrete_building_4.png)
![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Concrete_building_1.png)

## Scripting

This Plug-In supports the use of Grasshopper Scripting Components to deal with RF-Objects.

Two C# scripts are included in this file to show how to read properties of RF-Objects and how to create new RF-Objects or modify existing ones.

![alt text](https://lojmjq.db.files.1drv.com/y4m6mzkUGHsBKURoQEC9KryIe0ai1LUGBZKbO4tglJhGMNXb4E3x-0RNDS3scwy66Qcoy9mZFMbKUvIntx8-U9nw5Iu5EG0Wzx-DOHFwtpUaV_VYzd6TOAsELyaXfVUdyjSbCIGiLp0J_tzfdInZ83SqqTIR96-N2v4Yb960WBvG1RdfOcuB9cAuJn9izsy7Tx8rycITgh8yqXC78n4GzV4VQ?width=694&height=741&cropmode=none "Scripting - GH")

## Loads

The process of defining load data and load cases for an RFEM Modell inside Grasshopper is explained.
Some key aspects to take into account:

- It is possible to define model data, load data and load cases and combos within the same Set Data Component.  The export process to RFEM is faster this way since the connection between GH and RFEM is required only once.
- It can be very efficient to define theLive Load with chessboard distribution in Grasshopper.
- Get free polygon loads in grasshopper and assign a different color to the loads of each load case to enhance visualization

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Example5-GH.png)
![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Example5-RFEM.png)

## Extrude Members

Get 3d shapes of member elements from RFREM with the new "Extrude Members" component:

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Example6_ExtrudeMembers_1.png)
![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Example6_ExtrudeMembers_2.png)
![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Example6_ExtrudeMembers_3.png)

## Calculation

The "Get Results" component retrieves the results of the CALCULATED load cases and load combinations from RFEM. The output of the component is the deformed model shape and the calculation results of the selected load case and combo.

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Concrete_building_2.png)
![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Example7-Calculation.png)

## Optimization

Through this example, you will learn to:

- Edit existing RFEM Models
- Run the analysis of structural models from Grasshopper
- Run a cross section optimization from grasshopper
- Combine the cross section optimization with a geometric optimization using Galapagos

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Optimization_Galapagos.png)

## NURBS Surfaces

Through this example, you will learn to:

- Model Nurbs Surfaces in Grasshopper so they can be imported into RFEM
- Import Nurbs Surfaces into RFEM
- Import Nurbs Surfaces from RFEM into Grasshopper

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/NurbsSurfaces.png?raw=true)

## Life-Cycle Assessment

Through this example, you will learn to:

- Prepare the bill of quantities from the RFEM model to serve as input for a LCA
- Run a LCA with the plugin of One Click LCA with the input from an RFEM model

![alt text](https://github.com/diego-apellaniz/Parametric-FEM-Toolbox/blob/master/Images/Optimization_Galapagos.png)
