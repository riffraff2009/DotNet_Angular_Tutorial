import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit{
  @Input() member: Member | undefined;

  constructor(private memberService: MembersService, 
    private toastr: ToastrService,
    public presenceService: PresenceService 
    ){}
  
  ngOnInit(): void {
    
  }

  addFollow(member: Member){
    this.memberService.addFollow(member.userName).subscribe({
      next: ()=> this.toastr.success('You have followed ' + member.knownAs)
    })
  }

}
