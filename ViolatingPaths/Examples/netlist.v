module module_1 (in1, in2, out);
	input in1;
	input in2;
	output out;
	wire w;

	reg1 in1_reg ( .N(in1), .O(w));
	and1 and_in2_w ( .A(w), .B(in2), .C(out));
endmodule

module main (in1, in2, in3, in4, out);
	input in1;
	input in2;
	input in3;
	input in4;
	output out;
	
	wire w1, w2, a1, a2, b1;

	module_1 module1_in1_in2 ( .in1(in1), .in2(in2), .out(w1));
	module_1 module1_in2_in3 ( .in1(in2), .in2(in3), .out(w2));

	or1 or_w1_w2 ( .A(w1), .B(w2), .C(a1));
	and1 and_w2_in4 ( .A(w2), .B(in4), .C(a2));
	not1 not_a2 ( .A(a2), .B(b1));

	xor1 xor_a1_b1 ( .A(a1), .B(b1), .C(out));
endmodule