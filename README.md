# PLATEAU2VISSIM

国土交通省が主導するプロジェクト[PLATEAU](https://www.mlit.go.jp/plateau/)により整備、オープンデータ化されている3D都市モデルを、PTV Groupが開発・販売しているミクロ交通シミュレーター[PTV Vissim](https://www.ptvgroup.com/ja/%E3%82%BD%E3%83%AA%E3%83%A5%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3/%E8%A3%BD%E5%93%81/ptv-vissim/)にインポート可能なWavefront OBJ (.obj) 形式に自動で変換し、変換された3D都市モデルをVissim座標系の正しい座標にインポートします。  

本ツールは、ksasao様の[PlateauCityGmlSharp](https://github.com/ksasao/PlateauCityGmlSharp)というツールを改変し活用しています。   
※Project PLATEAU の 3D都市モデルで提供されている CityGML形式 (.gml) を Wavefront OBJ (.obj) 形式に変換するツール  

## ファイル構成
```txt
root  
　├ Importer  
　│　├ ***_appearance  
　│　├ ***.gml  
　│　├ ***_appearance  
　│　├ ***.gml  
　│　├ output  	※自動生成される    
　│　│　├ ***1  
　│　│　└ ***2  
　│　├ CityGMLToObj.exe  
　│　└ Importer.py  
　├ **.inpx  
　└ **.layx  
```  
### Importer.py：  
　Vissimから呼び出すPythonファイル。INPXファイルの基準点を取得し、緯度経度に変換したものを引数としてCityGMLToObj.exeを呼出す。  
　その後生成されたOBJファイルを一括でVissimにインポート。  

### CityGMLToObj.exe:  
　Project PLATEAU の 3D都市モデルで提供されている CityGML形式 (.gml) を Wavefront OBJ (.obj) 形式に変換。  
　ksasao様の公開されているツールを改変。　https://github.com/ksasao/PlateauCityGmlSharp  

#### オリジナルからの変更点
- Vissimの座標系に合わせて、XZの符号を反転させて180度回転（Position.cs：95行目～97行目）  
- 地表に建物を配置できるように標高をLowerCorner.Altitude分オフセット（ModelGenerator.cs：107行目）  


## 使い方  
#### ①対象の地域のCityGMLファイルをダウンロード。  
[G空間情報センター3D都市モデルポータルサイト](https://www.geospatial.jp/ckan/dataset/plateau)  
![1](https://user-images.githubusercontent.com/85535019/222119230-30e04046-968b-41c4-8610-48164664e6f0.png)  
#### 本ツールは、CityGML（v2）の形式には対応していませんのでご注意ください。
![image](https://user-images.githubusercontent.com/85535019/234492206-3537fecc-cbd5-4df4-8799-102c2930280c.png)
<br />

#### ②インポートしたい場所のファイルをImporterフォルダに解凍。  
　udx/bldgフォルダ下の、\**gmlファイルと\**appearanceフォルダ（テキスチャがある場合）  
　複数の場所に対応するファイルを入れてもよい（一括処理される）  
![2](https://user-images.githubusercontent.com/85535019/222120209-83e48a90-6664-4a30-87df-bebacbf370f2.png)  
![3](https://user-images.githubusercontent.com/85535019/222120289-63d0e52d-b9fe-4249-94cd-7492083deeb8.png)  
<br />

#### ③Vissimで新規ファイルを開き、Link等を作成して基準点を作成し、ファイルを保存。  
　あるいは既存のinpxファイルを開いてもよいが、基準点が離れていると誤差が大きくなる  
![4](https://user-images.githubusercontent.com/85535019/222120324-dc5d0593-db08-4c80-8ffb-1052dedd5372.png)  
![5](https://user-images.githubusercontent.com/85535019/222120317-22729888-0038-4042-b49a-897613b44d07.png)  
<br />

#### ④Pythonファイルの実行（Actions -> Run Script File… でImporter.pyを開く）。  
![6](https://user-images.githubusercontent.com/85535019/222120367-88dabf7d-d61f-4069-854a-ba1d5fe627f2.png)  
<br />

#### ⑤インポート完了まで数分（ファイルの数、サイズによる）待機する。  
![7](https://user-images.githubusercontent.com/85535019/222120408-ea9cb119-2d3c-47b7-8909-530fdf32fe6b.png)  

しばらくすると、「インポートが完了しました」と表示される。  
![8](https://user-images.githubusercontent.com/85535019/222120445-02a36163-4846-4612-9264-a638412ced5c.png)  

### Vissimにインポートされた3D都市モデル（例：渋谷）
![9](https://user-images.githubusercontent.com/85535019/222120654-9601402b-f215-44e7-9f8c-41b389db0837.png)  


## 動作環境  
- PTV Vissim 2023  
- Python 3.9 + pywin32  
※現在OBJファイルのインポートをPTV Vissimが公式にはサポートしていないため、今後の開発によっては仕様が変わり本ツールが利用できなくなる可能性があります。予めご了承ください。  


## ライセンス  
本ツールは、Apache License, Version 2.0に従います。  
https://www.apache.org/licenses/LICENSE-2.0  
（和訳：https://licenses.opensource.jp/Apache-2.0/Apache-2.0.html）  
PTV Vissimは、有償のソフトウェアです。[こちら](https://www.ptvgroup.com/ja/%E3%82%BD%E3%83%AA%E3%83%A5%E3%83%BC%E3%82%B7%E3%83%A7%E3%83%B3/%E8%A3%BD%E5%93%81/ptv-vissim/%E3%82%B3%E3%83%B3%E3%82%BF%E3%82%AF%E3%83%88/)からお問合せください。
