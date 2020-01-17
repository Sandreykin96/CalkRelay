Example 1: Calculatings

            PowerSystem pr = new PowerSystem();
            pr.PQNode = new List<PQNode>();
            pr.Grounding = new List<GroundingBranch>();
            pr.PVNode = new List<PVNode>();
            pr.Transformer = new List<Transformer>();
            pr.Transmission = new List<Transmission>();
            
            PQNode gen1 = new PQNode();
            gen1.Name = "FirstGE";
            gen1.Pg = 1;
            gen1.Qg = 0.6198;
            pr.PQNode = new List<PQNode>();
            pr.PQNode.Add(gen1);

            PQNode gen2 = new PQNode();
            gen2.Name = "SecondGEn";
            gen2.Pg = -2;
            gen2.Qg = -0.9687;
            pr.PQNode.Add(gen2);

            RelaxNode rel1 = new RelaxNode();
            rel1.Name = "StancA";
            rel1.U = 1;
            rel1.Delta = 0;
            pr.RelaxNode = rel1;

            PVNode gen3 = new PVNode();
            gen3.Name = "gen3";
            gen3.Pg = 0.96;
            gen3.U = 1.045;
            pr.PVNode.Add(gen3);

            Transmission tr1 = new Transmission();
            tr1.LeftNode = "StancA";
            tr1.RightNode = "FirstGE";
            tr1.Z = new System.Numerics.Complex(0.007747, 0.043388);
            pr.Transmission.Add(tr1);

            Transmission tr2 = new Transmission();
            tr2.LeftNode = "FirstGE";
            tr2.RightNode = "SecondGEn";
            tr2.Z = new System.Numerics.Complex(0.00619, 0.0347);
            pr.Transmission.Add(tr2);

            Transmission tr3 = new Transmission();
            tr3.LeftNode = "SecondGEn";
            tr3.RightNode = "gen3";
            tr3.Z = new System.Numerics.Complex(0.00543, 0.0303);
            pr.Transmission.Add(tr3);

            var s = pr.GetSolver<RectangularSolver>();
            s.GetSolve();
            var solu = new Solution(pr, s);

     
            Console.WriteLine("Loads: \n");
            foreach (var a in pr.PQNode)
            {
                Console.WriteLine("Node name = " + a.Name + " Active power = " + a.Pg + " Reactive power = " + a.Qg + "\n");
            }
			           
            Console.WriteLine("Calculaed nodes \n");

            foreach (var a in solu.Nodes)
            {
                Console.WriteLine("Name = " + a.Name + " Voltage = " + a.U + " Angle = " + a.Delta + " Active power = " + a.P.ToString().Substring(0, 5) + " Reactive power  = " + a.Q.ToString().Substring(0, 5) + "\n");
            }

            Console.WriteLine("Calculaed branches \n");
            foreach (var a in solu.PowerFlows)
            {
                Console.WriteLine("Left node = " + a.From + " Right node  = " + a.To + " Imagine current = " + a.Iim.ToString().Substring(0, 10) + " Real current = " + a.Ire.ToString().Substring(0, 10) + " Active power = " + a.P.ToString().Substring(0, 10) + " Reactive power  = " + a.Q.ToString().Substring(0, 10) + "\n");
            }

# Example 2: working with Json Files //dataFile - path to json file

        private static T Calculate<T>(string dataFile, string outJson)
            where T : NewtonSolwer, new()
        {
            var problem = JsonHelper.ReadJson(dataFile);
            var s = problem.GetSolver<T>();
            s.GetSolve();
            var solu = new Solution(problem, s);
            JsonHelper.WriteJson(outJson, solu);
            return s;
        }