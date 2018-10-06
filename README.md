Code base and resulting assembly for Assembly-CSharp.dll

There's not much going on with this branch; it just hosts the README at the moment.

There are three branches per arc, the most pertinent being `<arc>-mod`, the mod version of a given arc. The vanilla MG and reconstructed vanilla Steam (may not be exactly the same) versions of the game can be found for each arc at `<arc>-mg` and `<arc>-steam` respectively, and should be useful for comparison.

The branch lineage looks like this, where the parent of a branch at depth n is the nearest branch above at depth n-1; for example, the parent of `hima-mg` is `tata-mg`.

```
oni-mg
* oni-steam
** oni-mod
*** mod <-- development
**** f-lipsync <-- example feature in progress
* wata-mg
** wata-steam
*** wata-mod
** tata-mg
*** tata-steam
**** tata-mod
*** hima-mg
**** hima-steam
***** hima-mod
****** console-arcs
**** mea-mg
```

...etc. The rationale behind this is that each arc seems to build on the code from the previous arc, and the Steam versions seem to be extensions of the MG versions, while our mods are based on the Steam version.

Ongoing development will be based on the `mod` branch and the relevant branch for each arc is `<arc>-mod`, with the exception of `console-arcs`, which is branched off `hima-mod`.

These projects (`Assembly-CSharp.csproj`) have been built successfully with Visual Studio Community 2017 v15.7.5 and msbuild v15.7.180.61344, though in theory any compiler supporting C# 7.1 should suffice.

The built DLLs are available here:

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=oni-mod)](https://travis-ci.com/07th-mod/higurashi-assembly) [Onikakushi](https://07th-mod.com/higurashi_dlls/onikakushi/Assembly-CSharp.dll)

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=wata-mod)](https://travis-ci.com/07th-mod/higurashi-assembly) [Watanagashi](https://07th-mod.com/higurashi_dlls/watanagashi/Assembly-CSharp.dll)

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=tata-mod)](https://travis-ci.com/07th-mod/higurashi-assembly) [Tatarigoroshi](https://07th-mod.com/higurashi_dlls/tatarigoroshi/Assembly-CSharp.dll)

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=hima-mod)](https://travis-ci.com/07th-mod/higurashi-assembly) [Himatsubushi](https://07th-mod.com/higurashi_dlls/himatsubushi/Assembly-CSharp.dll)

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=mea-mod)](https://travis-ci.com/07th-mod/higurashi-assembly) [Meakashi](https://07th-mod.com/higurashi_dlls/meakashi/Assembly-CSharp.dll)

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=tsumi-mod)](https://travis-ci.com/07th-mod/higurashi-assembly) [Tsumihoroboshi](https://07th-mod.com/higurashi_dlls/tsumihoroboshi/Assembly-CSharp.dll)

[![Build Status](https://travis-ci.com/07th-mod/higurashi-assembly.svg?branch=console-arcs)](https://travis-ci.com/07th-mod/higurashi-assembly) [Console Arcs](https://07th-mod.com/higurashi_dlls/consolearcs/Assembly-CSharp.dll)

Other branches may not have up-to-date READMEs.
