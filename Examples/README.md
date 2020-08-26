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

## Non-Planar Surfaces

Non-planar surfaces can be defined and imported the same way as any other surface. However, there are some particular aspects to consider when dealing with this kind of geometry. The following concepts are explored:

- Dealing with BREPS -> They have to be deconstructed into single surfaces.
- How the Plug-In automatically sets planar surfaces as "PlaneSurfaceType" and non-planar suraces as "QuadrangleSurfaceType"
- Dealing with toroid geometries with duplicate edges. They have to be splited into two before being imported into RFEM.
- How to manually set the "SurfaceGeometryType" of RF-Objects
- Modifying existing RF-Objects INSIDE GRASSHOPPER

![alt text](https://nijmjq.db.files.1drv.com/y4mhBnePBHYd9yHulAO5A6ZWqbBn8leV_I5IVkvUY_X_GsPPd75A2Xh3MwRCL0ZPdOOdAIF_2ODoB_lKTo9WgX7u9eSOSOMOLj48RSIiRNRJ_8TkdjvzPHq32-OjRVqT5JGhJHoIyqSNuEfYEEdV2xtoUVj334zAn63f1GC3CS0J_hDEiVjaLjNCHThLWTwbHo8Nu7saH5Kr88JTZ9uRTII1A?width=889&height=440&cropmode=none "Non-Planar Surfaces - GH")
![alt text](https://l4jmjq.db.files.1drv.com/y4mGgmiAL3OSiq0D9lkWNDR3f8kAMXA9tpZK38Ae9sukMYEcD-RyCX4Vc193yvN1ckRNjGwAHdgbwpkDm8MWXfw3Wx7LqvUWcX7CDj1znutKR5qbSaxCWXp3-z5f1JId1CQu9cBN5xNH_OiV7DEeUedhFkrvo185s5I1OE8e9o5Lrkp-cTNZzpX1jRnoY_Vz4DT_24zUhz6jlE5hD3XhqD7rw?width=1684&height=715&cropmode=none "Non-Planar Surfaces - REFM")

## Scripting

This Plug-In supports the use of Grasshopper Scripting Components to deal with RF-Objects.

Two C# scripts are included in this file to show how to read properties of RF-Objects and how to create new RF-Objects or modify existing ones.

![alt text](https://lojmjq.db.files.1drv.com/y4m6mzkUGHsBKURoQEC9KryIe0ai1LUGBZKbO4tglJhGMNXb4E3x-0RNDS3scwy66Qcoy9mZFMbKUvIntx8-U9nw5Iu5EG0Wzx-DOHFwtpUaV_VYzd6TOAsELyaXfVUdyjSbCIGiLp0J_tzfdInZ83SqqTIR96-N2v4Yb960WBvG1RdfOcuB9cAuJn9izsy7Tx8rycITgh8yqXC78n4GzV4VQ?width=694&height=741&cropmode=none "Scripting - GH")