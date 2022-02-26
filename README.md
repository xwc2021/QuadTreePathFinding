# QuadTreePathFinding

quadtree path finding
[思路1](https://gpnnotes.blogspot.com/2018/10/quad-tree-path-finding-1.html)
[思路2](https://gpnnotes.blogspot.com/2018/10/1-quad-tree-path-finding-2.html)
[思路3](https://gpnnotes.blogspot.com/2018/10/1-quad-tree-path-finding-3.html)

![image](https://1.bp.blogspot.com/-p2NHWZxEdS0/W8wru7SU8lI/AAAAAAAAarE/N5I-0cfkqGoM8dUha3LVA2GlRF8NOwpLQCKgBGAs/s1600/modify3.png)

# 使用方法
打開Arena Scene找到CellMaker物件
點擊按鈕
[Get All BoxCollider & Generate QuadTree]
[TestPathFind]
就可以看到上面的結果

# 開關
showNodeLink = true
![image](https://lh3.googleusercontent.com/pw/AM-JKLWBr_Fe_xv_HOzAYEER7qLtcovBY2BoN7maMdnAMPueg2kCMBBt0z677ml3qdsWuSDPgo96Fu2avQooh6E5mllst_tQKRUCRMelfsIwqUglPSXGqWTCH8TSaWQ7KYh0bL-DdO42ZkQYWMBsne2iQUmxQw=w1367-h906-no?authuser=0)

# 比較

單純比較節點數量
不考慮單個QuadTreeConnectedNode的Link數
```
QuadTreeConnectedNode[] leftLink, rightLink, upLink, downLink;
```
| split level | QuadTreeConnectedNode (不含非leaf node) | QuadTreeConnectedNode (含非leaf node) | UniformGridNode |
|-------------|-----------------------------------------|---------------------------------------|-----------------|
| 2           | 16                                      | 21                                    | 16              |
| 3           | 55                                      | 73                                    | 64              |
| 4           | 130                                     | 173                                   | 256             |
| 5           | 304                                     | 405                                   | 1024            |
| 6           | 637                                     | 849                                   | 4096            |
| 7           | 1345                                    | 1793                                  | 16384           |
| 8           | 2821                                    | 3761                                  | 65536           |