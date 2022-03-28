module main (in1, in2, in3, in4, out1, out2);
	input in1;
	input in2;
	input in3;
	input in4;
	output out1;
	output out2;
	
	wire w1, a1, a2, b1, r1, r2;
	reg1 reg1_in1 ( .N(in1), .O(r1));
	or1 or_w1_w2 ( .A(r1), .B(in2), .C(w1));
	and1 and_w2_in4 ( .A(w1), .B(in3), .C(a2));
	not1 not_a2 ( .A(a2), .B(b1));
	reg2 reg2_b1 ( .N(b1), .O(r2));
	xor1 xor_a1_b1 ( .A(in4), .B(r2), .C(out1));
	reg2 reg2_b1_out ( .N(b1), .O(out2));
endmodule