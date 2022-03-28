module module_1 (in, out);
	input in;
	output out;
	wire w1, w2, w2, w3, w4, w5, w6, w7, w8, w9, w10, w11, w12;
	not1 not_in ( .A(in), .B(w1));
	or1 or_w1_in ( .A(w1), .B(in), .C(w2));
	xor1 xor_w2_w2 ( .A(w2), .B(w2), .C(w3));
	reg2 reg1_w3 ( .N(w3), .O(w4));
	not1 not_w4 ( .A(w4), .B(w5));
	and1 and_w5_in ( .A(w5), .B(in), .C(w6));
	reg2 reg2_w6 ( .N(w6), .O(w7));
	not1 not_w7 ( .A(w7), .B(w8));
	reg2 reg3_w8 ( .N(w8), .O(w9));
	not1 not_w9 ( .A(w9), .B(w10));
	not1 not_w10 ( .A(w10), .B(w11));
	not1 not_w11 ( .A(w11), .B(w12));
	reg2 reg4_w12 ( .N(w12), .O(out));	
endmodule

module module_2 (in1, in2, out);
	input in1, in2;
	output out;
	wire w1, w2;
	module_1 module1_in1 ( .in(in1), .out(w1));	
	module_1 module1_in2 ( .in(in2), .out(w2));	
	or1 or_w1_w2 ( .A(w1), .B(w2), .C(out));
endmodule



