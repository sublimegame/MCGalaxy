﻿/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Generator.Foilage {
    public sealed class AshTree : Tree {
        
		int halfHeight, branchAmount;
		const int widthMax = 5, branchHeightMax = 10, clusterSizeMax = 3;
        List<Vec3S32> branch = new List<Vec3S32>();
        
        public override int DefaultValue(Random rnd) { return rnd.Next(0, 11); }
        
        public override void SetData(Random rnd, int value) {
            this.rnd = rnd;           
            height = (byte)rnd.Next(5, 10);
			halfHeight = height/4;
			branchAmount = rnd.Next(10, 25);
        }
        
        public override void Generate(ushort x, ushort y, ushort z, TreeOutput output) {
        	// Do base trunk
            Vec3S32 p1 = new Vec3S32(x, y, z);
			Vec3S32 p2 = new Vec3S32(x, y + height, z);
			Line(p1, p2, output);
			
			for (int i = 0; i < branchAmount; i++) {
				DoBranch(x, y, z, output);
			}
        }
        
		
		void DoBranch(int x, int y, int z, TreeOutput output) {
			int dx = rnd.Next(-widthMax, widthMax);
			int dz = rnd.Next(-widthMax, widthMax);
			int clusterSize = rnd.Next(1, clusterSizeMax);
			int branchStart = rnd.Next(halfHeight, height);
			int branchMax = branchStart + rnd.Next(3, branchHeightMax);
			
			int R = clusterSize;
			Vec3S32[] marks = new [] { 
				new Vec3S32(x + dx - R, y + branchMax - R, z + dz - R), 
				new Vec3S32(x + dx + R, y + branchMax + R, z + dz + R) };
			
			DrawOp op = new EllipsoidDrawOp();
			Brush brush = new RandomBrush(new [] { new ExtBlock(Block.leaf, 0) });			
			op.SetMarks(marks);
			op.Perform(marks, brush, b => output(b.X, b.Y, b.Z, b.Block));
			
			Vec3S32 p1 = new Vec3S32(x, branchStart, z);
			Vec3S32 p2 = new Vec3S32(x + dx, y + branchMax, z + dz);
			Line(p1, p2, output);
		}
        
        void Line(Vec3S32 p1, Vec3S32 p2, TreeOutput output) {
            LineDrawOp.DrawLine(p1.X, p1.Y, p1.Z, 100, p2.X, p2.Y, p2.Z, branch);
            
            foreach (Vec3S32 P in branch) {
            	output((ushort)P.X, (ushort)P.Y, (ushort)P.Z, Block.trunk);
            }
            branch.Clear();
        }
    }
}