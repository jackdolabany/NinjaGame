<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.2" tiledversion="1.2.2" name="GameObjects" tilewidth="16" tileheight="16" tilecount="16" columns="4">
 <image source="GameObjects.png" width="64" height="64"/>
 <tile id="0">
  <properties>
   <property name="LoadClass" value="Player"/>
  </properties>
 </tile>
 <tile id="1">
  <properties>
   <property name="LoadClass" value="Enemy.Enemy1"/>
  </properties>
 </tile>
 <tile id="2">
  <properties>
   <property name="LoadClass" value="Chair"/>
  </properties>
 </tile>
 <tile id="3">
  <properties>
   <property name="LoadClass" value="Barrel"/>
  </properties>
 </tile>
 <tile id="4">
  <properties>
   <property name="LoadClass" value="Platform.StaticPlatform"/>
  </properties>
 </tile>
 <tile id="5">
  <properties>
   <property name="LoadClass" value="Platform.FallingPlatform"/>
  </properties>
 </tile>
</tileset>
